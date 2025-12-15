<template>
	<ion-page>
		<ion-tabs>
			<ion-router-outlet></ion-router-outlet>
			<ion-tab-bar slot="bottom">
				<ion-tab-button tab="timeline" href="/tabs/timeline">
					<ion-icon aria-hidden="true" :icon="time" />
					<ion-label>Timeline</ion-label>
				</ion-tab-button>

				<ion-tab-button tab="new-note" href="/tabs/new-note">
					<ion-icon aria-hidden="true" :icon="create" />
					<ion-label>New Entry</ion-label>
				</ion-tab-button>

				<ion-tab-button tab="photos" href="/tabs/photos">
					<ion-icon aria-hidden="true" :icon="images" />
					<ion-label>All Photos</ion-label>
				</ion-tab-button>
			</ion-tab-bar>
		</ion-tabs>
	</ion-page>
</template>

<script setup lang="ts">
import { IonTabBar, IonTabButton, IonTabs, IonLabel, IonIcon, IonPage, IonRouterOutlet } from '@ionic/vue';
import { time, create, images } from 'ionicons/icons';
</script>
