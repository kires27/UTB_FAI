<template>
	<ion-modal :is-open="isOpen" @will-dismiss="closeModal">
		<ion-header>
			<ion-toolbar>
				<ion-title>Photo Viewer</ion-title>
				<ion-buttons slot="end">
					<ion-button @click="closeModal">
						<ion-icon :icon="close" />
					</ion-button>
				</ion-buttons>
			</ion-toolbar>
		</ion-header>
		<ion-content class="photo-viewer-content">
			<div v-if="currentPhoto" class="photo-container">
				<img :src="photoSrc" :alt="photoPath" />
			</div>
			<div v-else class="loading-container">
				<ion-spinner />
				<p>Loading photo...</p>
			</div>
		</ion-content>
	</ion-modal>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import {
	IonModal,
	IonHeader,
	IonToolbar,
	IonTitle,
	IonButtons,
	IonButton,
	IonIcon,
	IonContent,
	IonSpinner
} from '@ionic/vue';
import { close } from 'ionicons/icons';
import { usePhotoGallery, UserPhoto } from '@/composables/usePhotoGallery';

const props = defineProps<{
	isOpen: boolean;
	photoPath: string;
}>();

const emit = defineEmits<{
	'update:isOpen': [value: boolean];
}>();

const { loadWebviewPath } = usePhotoGallery();

const currentPhoto = ref<UserPhoto | null>(null);
const photoSrc = ref<string>('');

const closeModal = () => {
	emit('update:isOpen', false);
};

const loadPhoto = async () => {
	if (!props.photoPath) return;

	try {
		const photo = await loadWebviewPath({ filepath: props.photoPath } as UserPhoto);
		currentPhoto.value = photo;
		photoSrc.value = photo.webviewPath || '';
	} catch (error) {
		console.error('Error loading photo for modal:', error);
		photoSrc.value = '';
	}
};

// Watch for photoPath changes and isOpen to load photo
watch([() => props.photoPath, () => props.isOpen], ([newPhotoPath, isOpen]) => {
	if (isOpen && newPhotoPath) {
		loadPhoto();
	}
}, { immediate: true });

// Reset when modal closes
watch(() => props.isOpen, (isOpen) => {
	if (!isOpen) {
		currentPhoto.value = null;
		photoSrc.value = '';
	}
});
</script>

<style scoped>
.photo-viewer-content {
	--background: rgba(0, 0, 0, 0.95);
}

.photo-container {
	display: flex;
	align-items: center;
	justify-content: center;
	height: 100%;
	padding: 20px;
}

.photo-container img {
	max-width: 100%;
	max-height: 100%;
	object-fit: contain;
	border-radius: 8px;
}

.loading-container {
	display: flex;
	flex-direction: column;
	align-items: center;
	justify-content: center;
	height: 100%;
	gap: 16px;
	color: white;
}

ion-modal {
	--background: rgba(0, 0, 0, 0.95);
}
</style>