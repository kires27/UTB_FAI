<template>
	<div class="photo-grid-container">
		<ion-header>
			<ion-toolbar>
				<ion-title>All Photos</ion-title>
				<ion-buttons slot="end">
					<ion-button @click="handleTakePhoto">
						<ion-icon :icon="camera" />
					</ion-button>
				</ion-buttons>
			</ion-toolbar>
		</ion-header>

		<ion-content>
			<div v-if="allPhotos.length > 0" class="photo-grid">
				<div v-for="photo in allPhotos" :key="photo.filepath" class="photo-item" @click="maximizePhoto(photo)">
					<img :src="photo.webviewPath" />
					<div class="photo-overlay">
						<ion-button fill="clear" size="small" class="delete-btn" @click.stop="deletePhoto(photo)">
							<ion-icon :icon="trash" />
						</ion-button>
					</div>
				</div>
			</div>

			<div v-else class="empty-state">
				<ion-icon :icon="images" class="empty-icon" />
				<h3>No Photos Yet</h3>
				<p>Start capturing memories by taking photos in the note creation tab</p>
				<ion-button fill="solid" @click="goToNotes">
					<ion-icon :icon="camera" slot="start" />
					Take First Photo
				</ion-button>
			</div>
		</ion-content>
	</div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, onUnmounted, onActivated } from 'vue';
import {
	IonContent,
	IonHeader,
	IonToolbar,
	IonTitle,
	IonButtons,
	IonButton,
	IonIcon,
	toastController
} from '@ionic/vue';
import { camera, images, trash } from 'ionicons/icons';
import { usePhotoGallery, UserPhoto } from '@/composables/usePhotoGallery';
import { useRouter } from 'vue-router';
import { useDailyEntries } from '@/composables/useDailyEntries';

const router = useRouter();
const { photos: galleryPhotos, takePhoto, deletePhoto: deleteGalleryPhoto, loadWebviewPath } = usePhotoGallery();
const { getAllPhotos, removePhotoFromNotes, entries } = useDailyEntries();

const allPhotos = ref<UserPhoto[]>([]);

	const loadAllPhotos = async () => {
		const photoPaths = getAllPhotos();

		const combinedPhotos = [
			...galleryPhotos.value,
			...photoPaths.map(path => ({ filepath: path } as UserPhoto))
		];

		const uniquePhotos = combinedPhotos.filter((photo, index, array) =>
			array.findIndex(p => p.filepath === photo.filepath) === index
		);

		const photosWithPaths = await Promise.all(
			uniquePhotos.map(async (photo) => {
				if (!photo.webviewPath) {
					return await loadWebviewPath(photo);
				}
				return photo;
			})
		);

		allPhotos.value = photosWithPaths;
	};

const deletePhoto = async (photo: UserPhoto) => {
	try {
		await deleteGalleryPhoto(photo);

		await removePhotoFromNotes(photo.filepath);

		await loadAllPhotos();
	} catch (error) {
		console.error('Error deleting photo:', error);
		showToast('Failed to delete photo');
	}
};

const handleTakePhoto = async () => {
	try {
		await takePhoto();
		await loadAllPhotos();
	} catch (error) {
		console.error('Error taking photo:', error);
		showToast('Failed to take photo');
	}
};

const maximizePhoto = (photo: UserPhoto) => {
	emit('maximizePhoto', photo);
};

const goToNotes = () => {
	router.push('/tabs/new-note');
};

const showToast = async (message: string) => {
	const toast = await toastController.create({
		message,
		duration: 2000,
		position: 'bottom'
	});
	await toast.present();
};

const emit = defineEmits<{
	maximizePhoto: [photo: UserPhoto];
}>();

watch(entries, () => {
	loadAllPhotos();
}, { deep: true });

onMounted(() => {
	loadAllPhotos();
});

</script>

<style scoped>
.photo-item:hover {
	transform: scale(1.05);
} 

</style>