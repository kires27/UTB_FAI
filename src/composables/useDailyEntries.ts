import { ref, onMounted, watch } from "vue";
import { Preferences } from "@capacitor/preferences";
import { DailyEntry, DailyEntriesData, Note } from "@/types";

const DAILY_ENTRIES_STORAGE = "daily-entries";

// Global state
const globalEntries = ref<DailyEntriesData>({ entries: {} });

export const useDailyEntries = () => {
	const entries = globalEntries;

	const getDateString = (date: Date = new Date()): string => {
		return date.toISOString().split("T")[0];
	};

	const getOrCreateEntry = (date: string): DailyEntry => {
		if (!entries.value.entries[date]) {
			entries.value.entries[date] = {
				date,
				notes: [],
				standalonePhotos: [],
			};
		}
		return entries.value.entries[date];
	};

	const saveNote = async (
		text: string,
		photoPaths: string[] = []
	): Promise<string> => {
		const date = getDateString();
		const entry = getOrCreateEntry(date);

		const note: Note = {
			id: Date.now().toString(),
			text,
			timestamp: Date.now(),
			photos: photoPaths,
		};

		entry.notes.push(note);
		await cacheEntries();
		return note.id;
	};

	const updateNote = async (
		noteId: string,
		text: string,
		photoPaths: string[]
	): Promise<void> => {
		for (const entry of Object.values(entries.value.entries)) {
			const note = entry.notes.find((n) => n.id === noteId);
			if (note) {
				note.text = text;
				note.photos = photoPaths;
				await cacheEntries();
				break;
			}
		}
	};

	const deleteNote = async (noteId: string): Promise<void> => {
		for (const entry of Object.values(entries.value.entries)) {
			const noteIndex = entry.notes.findIndex((n) => n.id === noteId);
			if (noteIndex !== -1) {
				entry.notes.splice(noteIndex, 1);
				await cacheEntries();
				break;
			}
		}
	};

	const addStandalonePhoto = async (
		photoPath: string,
		date?: string
	): Promise<void> => {
		const targetDate = date || getDateString();
		const entry = getOrCreateEntry(targetDate);
		entry.standalonePhotos.push(photoPath);
		await cacheEntries();
	};

	const removeStandalonePhoto = async (photoPath: string): Promise<void> => {
		for (const entry of Object.values(entries.value.entries)) {
			const photoIndex = entry.standalonePhotos.indexOf(photoPath);
			if (photoIndex !== -1) {
				entry.standalonePhotos.splice(photoIndex, 1);
				await cacheEntries();
				break;
			}
		}
	};

	const removePhotoFromNotes = async (photoPath: string): Promise<void> => {
		for (const entry of Object.values(entries.value.entries)) {
			for (const note of entry.notes) {
				const photoIndex = note.photos.indexOf(photoPath);
				if (photoIndex !== -1) {
					note.photos.splice(photoIndex, 1);
					await cacheEntries();
					console.log("Removed photo from note:", photoPath);
					return;
				}
			}
		}
	};

	const getEntriesByDateRange = (
		startDate: string,
		endDate: string
	): DailyEntry[] => {
		const sortedEntries: DailyEntry[] = [];

		for (const date in entries.value.entries) {
			if (date >= startDate && date <= endDate) {
				const entry = entries.value.entries[date];
				entry.notes.sort((a, b) => b.timestamp - a.timestamp);
				sortedEntries.push(entry);
			}
		}

		// newest first
		return sortedEntries.sort((a, b) => b.date.localeCompare(a.date));
	};

	const getAllPhotos = (): string[] => {
		const allPhotos: string[] = [];

		for (const entry of Object.values(entries.value.entries)) {
			allPhotos.push(...entry.standalonePhotos);

			for (const note of entry.notes) {
				allPhotos.push(...note.photos);
			}
		}

		return allPhotos;
	};

	const getEntry = (date: string): DailyEntry | null => {
		return entries.value.entries[date] || null;
	};

	const getNoteById = (noteId: string): Note | null => {
		for (const entry of Object.values(entries.value.entries)) {
			const note = entry.notes.find((n) => n.id === noteId);
			if (note) return note;
		}
		return null;
	};

	const cacheEntries = async () => {
		await Preferences.set({
			key: DAILY_ENTRIES_STORAGE,
			value: JSON.stringify(entries.value),
		});
	};

	const loadEntries = async () => {
		const entriesData = await Preferences.get({ key: DAILY_ENTRIES_STORAGE });
		if (entriesData.value) {
			entries.value = JSON.parse(entriesData.value);
		}
	};

	onMounted(loadEntries);
	watch(entries, cacheEntries, { deep: true });

	const refreshEntries = async () => {
		await loadEntries();
	};

	return {
		entries,
		saveNote,
		updateNote,
		deleteNote,
		addStandalonePhoto,
		removeStandalonePhoto,
		removePhotoFromNotes,
		getEntriesByDateRange,
		getAllPhotos,
		getEntry,
		getNoteById,
		getDateString,
		refreshEntries,
	};
};
