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
import { computed, ref, onMounted, onActivated, onUnmounted, watch } from 'vue';
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
const { photos: galleryPhotos, takePhoto, deletePhoto: deleteGalleryPhoto, loadWebviewPath, takePhotoForEntry } = usePhotoGallery();
const { getAllPhotos, removePhotoFromNotes, entries } = useDailyEntries();

const allPhotos = ref<UserPhoto[]>([]);

const loadAllPhotos = async () => {
	// Get all photo paths from daily entries
	const photoPaths = getAllPhotos();

	// Combine with standalone gallery photos
	const combinedPhotos = [
		...galleryPhotos.value,
		...photoPaths.map(path => ({ filepath: path } as UserPhoto))
	];

	// Remove duplicates
	const uniquePhotos = combinedPhotos.filter((photo, index, array) =>
		array.findIndex(p => p.filepath === photo.filepath) === index
	);

	// Load webview paths for all photos
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
		// Try to delete from gallery photos first
		await deleteGalleryPhoto(photo);

		// Also try to remove from notes
		await removePhotoFromNotes(photo.filepath);

		// Reload all photos
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

// Watch for changes in gallery photos and reload
watch(galleryPhotos, () => {
	loadAllPhotos();
}, { deep: true });

// Watch for changes in entries (new photos added from notes)
watch(entries, () => {
	loadAllPhotos();
}, { deep: true });

// Refresh when component becomes active (tab navigation)
onActivated(() => {
	loadAllPhotos();
});

// Load photos when component mounts
onMounted(() => {
	loadAllPhotos();
});

// Add periodic refresh to catch any missed changes
let refreshInterval: NodeJS.Timeout;
const startPeriodicRefresh = () => {
	refreshInterval = setInterval(() => {
		loadAllPhotos();
	}, 2000); // Check every 2 seconds
};

// Clear interval when component is unmounted
const cleanup = () => {
	if (refreshInterval) {
		clearInterval(refreshInterval);
	}
};

// Start periodic refresh and set cleanup
onMounted(() => {
	startPeriodicRefresh();
	window.addEventListener('beforeunload', cleanup);
});

// Use Vue's onUnmounted instead of beforeunload for better lifecycle
onUnmounted(() => {
	cleanup();
});
</script>

<style scoped>
.photo-grid-container {
	height: 100vh;
	display: flex;
	flex-direction: column;
}

.photo-grid {
	display: grid;
	grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
	gap: 8px;
	padding: 8px;
}

.photo-item {
	position: relative;
	aspect-ratio: 1;
	border-radius: 8px;
	overflow: hidden;
	cursor: pointer;
	transition: transform 0.2s;
}

.photo-item:hover {
	transform: scale(1.05);
}

.photo-item img {
	width: 100%;
	height: 100%;
	object-fit: cover;
}

.photo-overlay {
	position: absolute;
	top: 0;
	left: 0;
	right: 0;
	bottom: 0;
	background: linear-gradient(to bottom, transparent 60%, rgba(0, 0, 0, 0.5));
	opacity: 0;
	transition: opacity 0.2s;
	display: flex;
	align-items: flex-end;
	justify-content: flex-end;
	padding: 8px;
}

.photo-item:hover .photo-overlay {
	opacity: 1;
}

.delete-btn {
	--background: rgba(239, 68, 68, 0.9);
	--color: white;
	--border-radius: 50%;
	width: 36px;
	height: 36px;
}

.empty-state {
	display: flex;
	flex-direction: column;
	align-items: center;
	justify-content: center;
	height: 60vh;
	text-align: center;
	padding: 0 32px;
}

.empty-icon {
	font-size: 4rem;
	color: var(--ion-color-medium);
	margin-bottom: 16px;
}

.empty-state h3 {
	margin: 0 0 8px 0;
	color: var(--ion-color-dark);
}

.empty-state p {
	margin: 0 0 24px 0;
	color: var(--ion-color-medium);
	line-height: 1.4;
}
</style>