<template>
	<ion-page>
		<PhotoGrid @maximize-photo="openPhotoViewer" />

		<MediaViewer :is-open="isPhotoViewerOpen" :photo-path="selectedPhotoPath" @update:is-open="closePhotoViewer" />
	</ion-page>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { IonPage } from '@ionic/vue';
import PhotoGrid from '@/components/PhotoGrid.vue';
import MediaViewer from '@/components/MediaViewer.vue';
import { UserPhoto } from '@/types';

const isPhotoViewerOpen = ref(false);
const selectedPhotoPath = ref('');

const openPhotoViewer = (photo: UserPhoto) => {
	selectedPhotoPath.value = photo.filepath;
	isPhotoViewerOpen.value = true;
};

const closePhotoViewer = () => {
	isPhotoViewerOpen.value = false;
	selectedPhotoPath.value = '';
};
</script>
