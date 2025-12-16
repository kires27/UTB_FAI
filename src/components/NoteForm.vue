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
				<ion-textarea v-model="noteText" placeholder="Write your thoughts here..." :auto-grow="true" :rows="8"
					class="note-textarea" />

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

		if (error.message) {
			showToast(error.message as string);
		}
	}
};

const removePhoto = (index: number) => {
	photos.value.splice(index, 1);
};

const handleSaveNote = async () => {
	try {
		await saveDailyEntry(noteText.value.trim(), photos.value.map(p => p.filepath));

		noteText.value = '';
		photos.value = [];

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
	--padding-top: 24px;
}

</style>