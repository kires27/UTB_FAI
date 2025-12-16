<template>
	<div class="timeline-container">
		<div v-for="entry in timelineEntries" :key="entry.date" class="day-section">
			<div class="date-header">
				<h2>{{ formatDate(entry.date) }}</h2>
			</div>

			<div class="content-container">
				<div v-for="note in entry.notes" :key="note.id" class="note-card" @click="showNoteActions(note)">
					<div class="note-content">
						<div class="note-text">
							<p class="preserve-lines">{{ note.text.substring(0, 200) }}{{ note.text.length > 200 ? '...'
								: '' }}</p>
							<div class="note-time">{{ formatTime(note.timestamp) }}</div>
						</div>

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

				<!-- <div v-for="photo in entry.standalonePhotos" :key="photo" class="photo-card">
					<img :src="photoSrcMap[photo] || ''" @click="maximizePhoto(photo)" />
				</div> -->
			</div>
		</div>

		<div v-if="timelineEntries.length === 0" class="empty-state">
			<p>No entries yet. Start creating notes in the second tab!</p>
		</div>
	</div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted, watch } from 'vue';
import {
	alertController,
	toastController
} from '@ionic/vue';
import { Capacitor } from '@capacitor/core';
import { useDailyEntries } from '@/composables/useDailyEntries';
import { usePhotoGallery } from '@/composables/usePhotoGallery';
import { Note, UserPhoto } from '@/types';

const { getEntriesByDateRange, updateNote, deleteNote, removePhotoFromNotes } = useDailyEntries();
const { loadWebviewPath } = usePhotoGallery();

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

	for (const entry of timelineEntries.value) {
		entry.standalonePhotos.forEach(photo => allPhotoPaths.add(photo));
		entry.notes.forEach(note => {
			note.photos.forEach(photo => allPhotoPaths.add(photo));
		});
	}

	const loadPromises = Array.from(allPhotoPaths).map(async (photoPath) => {
		try {
			const photo = await loadWebviewPath({ filepath: photoPath } as UserPhoto);
			photoSrcMap.value[photoPath] = photo.webviewPath || photoPath;
		} catch (error) {
			console.error('Error loading photo:', photoPath, error);
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

watch(timelineEntries, () => {
	loadPhotosForEntries();
}, { deep: true });

// onActivated(() => {
// 	loadPhotosForEntries();
// });

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
