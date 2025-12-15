<template>
	<div class="timeline-container">
		<div v-for="entry in timelineEntries" :key="entry.date" class="day-section">
			<div class="date-header">
				<h2>{{ formatDate(entry.date) }}</h2>
			</div>

			<div class="content-container">
				<!-- Notes -->
				<div v-for="note in entry.notes" :key="note.id" class="note-card" @click="showNoteActions(note)">
					<div class="note-content">
						<div class="note-text">
							<p class="preserve-lines">{{ note.text.substring(0, 200) }}{{ note.text.length > 200 ? '...' : '' }}</p>
							<div class="note-time">{{ formatTime(note.timestamp) }}</div>
						</div>

						<!-- Photos in note -->
						<div v-if="note.photos.length > 0" class="note-photos">
							<img v-for="(photo, index) in note.photos.slice(0, 3)" :key="index"
								:src="photoSrcMap[photo] || ''" class="note-photo-thumbnail"
								@click.stop="maximizePhoto(photo)" />
							<div v-if="note.photos.length > 3" class="more-photos">
								+{{ note.photos.length - 3 }}
							</div>
						</div>
					</div>
				</div>

				<!-- Standalone photos -->
				<div v-for="photo in entry.standalonePhotos" :key="photo" class="photo-card">
					<img :src="photoSrcMap[photo] || ''" @click="maximizePhoto(photo)" />
				</div>
			</div>
		</div>

		<div v-if="timelineEntries.length === 0" class="empty-state">
			<p>No entries yet. Start creating notes in the second tab!</p>
		</div>
	</div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted, watch, onActivated, onUnmounted } from 'vue';
import {
	alertController,
	toastController
} from '@ionic/vue';
import { create, trash } from 'ionicons/icons';
import { Capacitor } from '@capacitor/core';
import { useDailyEntries } from '@/composables/useDailyEntries';
import { usePhotoGallery } from '@/composables/usePhotoGallery';
import { Note, UserPhoto } from '@/types';

const { entries, getEntriesByDateRange, updateNote, deleteNote, removePhotoFromNotes } = useDailyEntries();
const { loadWebviewPath } = usePhotoGallery();

const loadedPhotos = ref<Map<string, string>>(new Map());

const timelineEntries = computed(() => {
	const today = new Date();
	const thirtyDaysAgo = new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000);

	return getEntriesByDateRange(
		thirtyDaysAgo.toISOString().split('T')[0],
		today.toISOString().split('T')[0]
	);
});

const photoSrcMap = ref<Record<string, string>>({});

const loadPhotosForEntries = async () => {
	const allPhotoPaths = new Set<string>();

	// Collect all photo paths from entries
	for (const entry of timelineEntries.value) {
		entry.standalonePhotos.forEach(photo => allPhotoPaths.add(photo));
		entry.notes.forEach(note => {
			note.photos.forEach(photo => allPhotoPaths.add(photo));
		});
	}

	// Load all photos
	const loadPromises = Array.from(allPhotoPaths).map(async (photoPath) => {
		try {
			// First try to load from filesystem
			const photo = await loadWebviewPath({ filepath: photoPath } as UserPhoto);
			photoSrcMap.value[photoPath] = photo.webviewPath || photoPath;
		} catch (error) {
			console.error('Error loading photo:', photoPath, error);
			// Fallback: try Capacitor.convertFileSrc for hybrid platforms
			try {
				const webviewPath = Capacitor.convertFileSrc(photoPath);
				photoSrcMap.value[photoPath] = webviewPath;
			} catch (fallbackError) {
				console.error('Fallback failed for photo:', photoPath, fallbackError);
				photoSrcMap.value[photoPath] = '';
			}
		}
	});

	await Promise.all(loadPromises);
};

const showNoteActions = async (note: Note) => {
	const actionSheet = await alertController.create({
		header: 'Note Options',
		message: 'What would you like to do with this note?',
		buttons: [
			{
				text: 'Edit Note',
				handler: async () => {
					const alert = await alertController.create({
						header: 'Edit Note',
						inputs: [
							{
								name: 'noteText',
								type: 'textarea',
								placeholder: 'Enter your note...',
								value: note.text,
							}
						],
						buttons: [
							{
								text: 'Cancel',
								role: 'cancel',
								handler: () => { }
							},
							{
								text: 'Save',
								handler: async (data) => {
									try {
										await updateNote(note.id, data.noteText || '', note.photos);
									} catch (error) {
										console.error('Error updating note:', error);
										showToast('Failed to update note');
									}
								}
							}
						]
					});
					await alert.present();
				}
			},
			{
				text: 'Remove Image',
				handler: async () => {
					if (note.photos.length > 0) {
						const alert = await alertController.create({
							header: 'Remove Image',
							message: 'Select an image to remove:',
							inputs: note.photos.map((photo, index) => {
								const fileName = photo.split('/').pop() || `photo_${index + 1}`;
								return {
									name: 'selectedPhoto', // Use consistent name for radio group
									type: 'radio',
									label: `${index + 1}: ${fileName}`,
									value: photo,
									checked: index === 0
								} as any;
							}),
							buttons: [
								{
									text: 'Cancel',
									role: 'cancel',
									handler: () => { }
								},
								{
									text: 'Remove',
									role: 'destructive',
									handler: async (data: any) => {
										try {
											console.log('Alert handler data:', data);
											const photoToRemove = data as string;
											console.log('Photo to remove:', photoToRemove);

											if (photoToRemove && typeof photoToRemove === 'string') {
												await removePhotoFromNotes(photoToRemove);
												await loadPhotosForEntries();
											} else {
												console.log('No valid photo selected, data:', data);
												showToast('No image selected for removal');
											}
										} catch (error) {
											console.error('Error removing image:', error);
											showToast('Failed to remove image');
										}
									}
								}
							]
						});
						await alert.present();
					} else {
						showToast('No images to remove');
					}
				}
			},
			{
				text: 'Delete Note',
				role: 'destructive',
				handler: async () => {
					try {
						await deleteNote(note.id);
					} catch (error) {
						console.error('Error deleting note:', error);
						showToast('Failed to delete note');
					}
				}
			},
			{
				text: 'Cancel',
				role: 'cancel',
			}
		]
	});
	await actionSheet.present();
};

const maximizePhoto = (photoPath: string) => {
	emit('maximizePhoto', photoPath);
};

const showToast = async (message: string) => {
	const toast = await toastController.create({
		message,
		duration: 2000,
		position: 'bottom'
	});
	await toast.present();
};

// Watch for changes in entries to reload photos
watch(timelineEntries, () => {
	loadPhotosForEntries();
}, { deep: true });

// Also refresh when component becomes active (tab navigation)
onActivated(() => {
	loadPhotosForEntries();
});

onMounted(() => {
	loadPhotosForEntries();
});

const emit = defineEmits<{
	maximizePhoto: [photoPath: string];
}>();

const formatDate = (dateString: string): string => {
	const date = new Date(dateString);
	return date.toLocaleDateString('en-US', {
		weekday: 'long',
		year: 'numeric',
		month: 'long',
		day: 'numeric'
	});
};

const formatTime = (timestamp: number): string => {
	return new Date(timestamp).toLocaleTimeString('en-US', {
		hour: '2-digit',
		minute: '2-digit'
	});
};
</script>

<style scoped>
.timeline-container {
	padding: 16px;
}

.day-section {
	margin-bottom: 32px;
}

.date-header {
	position: sticky;
	top: 0;
	background: var(--ion-background-color);
	padding: 8px 0;
	border-bottom: 1px solid var(--ion-color-light);
	margin-bottom: 16px;
	z-index: 10;
}

.date-header h2 {
	margin: 0;
	color: var(--ion-color-primary);
	font-size: 1.2rem;
	font-weight: 600;
}

.content-container {
	display: flex;
	flex-direction: column;
	gap: 12px;
}

.note-card {
	background: var(--ion-card-background);
	border-radius: 12px;
	padding: 16px;
	box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
	cursor: pointer;
	transition: transform 0.2s, box-shadow 0.2s;
}

.note-card:hover {
	transform: translateY(-2px);
	box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.note-content {
	display: flex;
	gap: 16px;
	align-items: flex-start;
}

.note-text {
	flex: 1;
	font-size: 0.95rem;
	line-height: 1.4;
	color: var(--ion-color-dark);
}

.note-text p {
	margin: 0 0 8px 0;
}

.note-text .preserve-lines {
	white-space: pre-line;
	line-height: 1.4;
}

.note-time {
	font-size: 0.8rem;
	color: var(--ion-color-medium);
}

.note-photos {
	display: flex;
	gap: 8px;
	align-items: center;
	flex-wrap: wrap;
}

.note-photo-thumbnail {
	width: 60px;
	height: 60px;
	object-fit: cover;
	border-radius: 8px;
	cursor: pointer;
}

.more-photos {
	width: 60px;
	height: 60px;
	background: var(--ion-color-light);
	border-radius: 8px;
	display: flex;
	align-items: center;
	justify-content: center;
	font-size: 0.8rem;
	color: var(--ion-color-medium);
}

.photo-card {
	border-radius: 12px;
	overflow: hidden;
	box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
	cursor: pointer;
	transition: transform 0.2s, box-shadow 0.2s;
}

.photo-card:hover {
	transform: translateY(-2px);
	box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.photo-card img {
	width: 100%;
	height: 200px;
	object-fit: cover;
}

.empty-state {
	text-align: center;
	padding: 48px 16px;
	color: var(--ion-color-medium);
}

.empty-state p {
	font-size: 1rem;
	margin: 0;
}
</style>