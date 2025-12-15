<template>
	<ion-page>
		<ion-header>
			<ion-toolbar>
				<ion-title>Daily Timeline</ion-title>
			</ion-toolbar>
		</ion-header>
		<ion-content>
			<TimelineView @maximize-note="maximizeNote" @maximize-photo="openPhotoViewer" />

			<MediaViewer :is-open="isPhotoViewerOpen" :photo-path="selectedPhotoPath"
				@update:is-open="closePhotoViewer" />
		</ion-content>
	</ion-page>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { IonPage, IonHeader, IonToolbar, IonTitle, IonContent } from '@ionic/vue';
import TimelineView from '@/components/TimelineView.vue';
import MediaViewer from '@/components/MediaViewer.vue';
import { Note } from '@/types';

const isPhotoViewerOpen = ref(false);
const selectedPhotoPath = ref('');

const maximizeNote = (note: Note) => {
	console.log('Maximize note:', note);
	// TODO: Implement note maximization modal
};

const openPhotoViewer = (photoPath: string) => {
	selectedPhotoPath.value = photoPath;
	isPhotoViewerOpen.value = true;
};

const closePhotoViewer = () => {
	isPhotoViewerOpen.value = false;
	selectedPhotoPath.value = '';
};
</script>
