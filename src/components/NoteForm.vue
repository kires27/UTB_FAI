<template>
	<div class="note-form-container">
		<ion-header>
			<ion-toolbar>
				<ion-buttons slot="start">
					<ion-back-button default-href="/tabs/timeline"></ion-back-button>
				</ion-buttons>
				<ion-title>New Note</ion-title>
				<ion-buttons slot="end">
					<ion-button @click="handleSaveNote" :disabled="!noteText.trim() && photos.length === 0">
						<ion-icon :icon="arrowForward" />
					</ion-button>
				</ion-buttons>
			</ion-toolbar>
		</ion-header>

		<ion-content class="ion-padding">
			<div class="form-content">
				<!-- Text input area -->
				<ion-textarea v-model="noteText" placeholder="Write your thoughts here..." :auto-grow="true" :rows="8"
					class="note-textarea" />

				<!-- Photo preview area -->
				<div v-if="photos.length > 0" class="photo-preview-container">
					<h4>Attached Photos</h4>
					<div class="photo-preview-grid">
						<div v-for="(photo, index) in photos" :key="index" class="photo-preview-item">
							<img :src="photo.webviewPath" />
							<ion-button fill="clear" size="small" class="remove-photo-btn" @click="removePhoto(index)">
								<ion-icon :icon="close" />
							</ion-button>
						</div>
					</div>
				</div>

				<!-- Photo capture buttons -->
				<div class="photo-actions">
					<ion-button expand="block" fill="outline" @click="takePhoto" class="photo-btn">
						<ion-icon :icon="camera" slot="start" />
						Take Photo
					</ion-button>

					<ion-button expand="block" fill="outline" @click="importPhoto" class="photo-btn">
						<ion-icon :icon="images" slot="start" />
						Import Photo
					</ion-button>
				</div>
			</div>
		</ion-content>
	</div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import {
	IonContent,
	IonHeader,
	IonToolbar,
	IonTitle,
	IonButtons,
	IonButton,
	IonIcon,
	IonTextarea,
	IonBackButton,
	toastController
} from '@ionic/vue';
import { arrowForward, camera, images, close } from 'ionicons/icons';
import { useDailyEntries } from '@/composables/useDailyEntries';
import { usePhotoGallery, UserPhoto } from '@/composables/usePhotoGallery';
import { useRouter } from 'vue-router';

const router = useRouter();
const { saveNote: saveDailyEntry } = useDailyEntries();
const { takePhotoForEntry, importPhoto: importPhotoFromGallery } = usePhotoGallery();

const noteText = ref('');
const photos = ref<UserPhoto[]>([]);

const takePhoto = async () => {
	try {
		const photo = await takePhotoForEntry();
		photos.value.push(photo);
	} catch (error) {
		console.error('Error taking photo:', error);
		showToast('Failed to take photo');
	}
};

const importPhoto = async () => {
	try {
		const photo = await importPhotoFromGallery();
		photos.value.push(photo);
	} catch (error: any) {
		console.error('Error importing photo:', error);

		let errorMessage = 'Failed to import photo';
		if (error.message && error.message.includes('Only JPG and PNG images are allowed')) {
			errorMessage = 'Only JPG and PNG images are allowed';
		} else if (error.message && error.message.includes('User cancelled')) {
			errorMessage = 'Photo selection cancelled';
		}

		showToast(errorMessage);
	}
};

const removePhoto = (index: number) => {
	photos.value.splice(index, 1);
};

const handleSaveNote = async () => {
	try {
		const noteId = await saveDailyEntry(noteText.value.trim(), photos.value.map(p => p.filepath));

		// Reset form
		noteText.value = '';
		photos.value = [];

		// Navigate back to timeline
		router.push('/tabs/timeline');
	} catch (error) {
		console.error('Error saving note:', error);
		showToast('Failed to save entry');
	}
};

const showToast = async (message: string) => {
	const toast = await toastController.create({
		message,
		duration: 2000,
		position: 'bottom'
	});
	await toast.present();
};
</script>

<style scoped>
.note-form-container {
	height: 100vh;
	display: flex;
	flex-direction: column;
}

.form-content {
	display: flex;
	flex-direction: column;
	gap: 20px;
}

.note-textarea {
	--background: var(--ion-color-light);
	--border-radius: 12px;
	--padding-start: 16px;
	--padding-end: 16px;
	--padding-top: 16px;
	--padding-bottom: 16px;
}

.photo-preview-container {
	margin-top: 16px;
}

.photo-preview-container h4 {
	margin: 0 0 12px 0;
	color: var(--ion-color-dark);
	font-size: 1rem;
}

.photo-preview-grid {
	display: grid;
	grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
	gap: 12px;
}

.photo-preview-item {
	position: relative;
	aspect-ratio: 1;
	border-radius: 8px;
	overflow: hidden;
}

.photo-preview-item img {
	width: 100%;
	height: 100%;
	object-fit: cover;
}

.remove-photo-btn {
	position: absolute;
	top: 4px;
	right: 4px;
	--background: rgba(0, 0, 0, 0.7);
	--color: white;
	--border-radius: 50%;
	width: 32px;
	height: 32px;
}

.photo-actions {
	display: flex;
	flex-direction: column;
	gap: 12px;
	margin-top: 16px;
}

.photo-btn {
	--border-radius: 12px;
}
</style>