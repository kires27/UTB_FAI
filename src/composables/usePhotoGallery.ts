import { ref, onMounted, watch } from "vue";
import {
	Camera,
	CameraResultType,
	CameraSource,
	Photo,
} from "@capacitor/camera";
import { Filesystem, Directory } from "@capacitor/filesystem";
import { Preferences } from "@capacitor/preferences";
import { isPlatform } from "@ionic/vue";
import { Capacitor } from "@capacitor/core";
import { UserPhoto } from "@/types";

const PHOTO_STORAGE = "photos";

export type { UserPhoto };

export const usePhotoGallery = () => {
	const photos = ref<UserPhoto[]>([]);

	const convertBlobToBase64 = (blob: Blob) =>
		new Promise((resolve, reject) => {
			const reader = new FileReader();
			reader.onerror = reject;
			reader.onload = () => {
				resolve(reader.result as string);
			};
			reader.readAsDataURL(blob);
		});

	const savePicture = async (
		photo: Photo,
		fileName: string
	): Promise<UserPhoto> => {
		let base64Data: string;

		if (isPlatform("hybrid")) {
			const file = await Filesystem.readFile({
				path: photo.path!,
			});
			base64Data = file.data as string;
		} else {
			const response = await fetch(photo.webPath!);
			const blob = await response.blob();
			base64Data = (await convertBlobToBase64(blob)) as string;
		}

		const savedFile = await Filesystem.writeFile({
			path: fileName,
			data: base64Data,
			directory: Directory.Data,
		});

		if (isPlatform("hybrid")) {
			const webviewPath = Capacitor.convertFileSrc(savedFile.uri);
			console.log("Hybrid photo saved:", {
				filepath: savedFile.uri,
				webviewPath,
			});
			return {
				filepath: savedFile.uri,
				webviewPath: webviewPath,
				timestamp: Date.now(),
			};
		} else {
			console.log("Web photo saved:", {
				filepath: fileName,
				webviewPath: photo.webPath,
			});
			return {
				filepath: fileName,
				webviewPath: photo.webPath,
				timestamp: Date.now(),
			};
		}
	};

	const takePhoto = async () => {
		const photo = await Camera.getPhoto({
			resultType: CameraResultType.Uri,
			source: CameraSource.Camera,
			quality: 100,
		});

		const fileName = Date.now() + ".jpeg";
		const savedFileImage = await savePicture(photo, fileName);
		photos.value = [savedFileImage, ...photos.value];
	};

	const cachePhotos = () => {
		Preferences.set({
			key: PHOTO_STORAGE,
			value: JSON.stringify(photos.value),
		});
	};

	const loadSaved = async () => {
		const photoList = await Preferences.get({ key: PHOTO_STORAGE });
		const photosInPreferences = photoList.value
			? JSON.parse(photoList.value)
			: [];

		for (const photo of photosInPreferences) {
			if (!photo.webviewPath) {
				try {
					if (isPlatform("hybrid")) {
						photo.webviewPath = Capacitor.convertFileSrc(photo.filepath);
						console.log("Loaded hybrid photo webviewPath:", photo.webviewPath);
					} else {
						const file = await Filesystem.readFile({
							path: photo.filepath,
							directory: Directory.Data,
						});
						photo.webviewPath = `data:image/jpeg;base64,${file.data}`;
						console.log("Loaded web photo webviewPath from base64");
					}
				} catch (error) {
					console.error(
						"Error loading webviewPath for photo:",
						photo.filepath,
						error
					);
					try {
						const file = await Filesystem.readFile({
							path: photo.filepath,
						});
						photo.webviewPath = `data:image/jpeg;base64,${file.data}`;
					} catch (fallbackError) {
						console.error(
							"Fallback failed for photo:",
							photo.filepath,
							fallbackError
						);
						photo.webviewPath = "";
					}
				}
			}
		}

		photos.value = photosInPreferences;
	};

	const deletePhoto = async (photo: UserPhoto) => {
		photos.value = photos.value.filter((p) => p.filepath !== photo.filepath);

		const filename = photo.filepath.substring(
			photo.filepath.lastIndexOf("/") + 1
		);
		await Filesystem.deleteFile({
			path: filename,
			directory: Directory.Data,
		});
	};

	watch(photos, cachePhotos);

	onMounted(loadSaved);

	const takePhotoForEntry = async (date?: string): Promise<UserPhoto> => {
		const photo = await Camera.getPhoto({
			resultType: CameraResultType.Uri,
			source: CameraSource.Camera,
			quality: 100,
		});

		const fileName = Date.now() + ".jpeg";
		const savedFileImage = await savePicture(photo, fileName);

		if (!date) {
			photos.value = [savedFileImage, ...photos.value];
		}

		return savedFileImage;
	};

	const loadWebviewPath = async (photo: UserPhoto): Promise<UserPhoto> => {
		if (!photo.webviewPath) {
			try {
				const file = await Filesystem.readFile({
					path: photo.filepath,
					directory: Directory.Data,
				});
				return {
					...photo,
					webviewPath: `data:image/jpeg;base64,${file.data}`,
				};
			} catch (error) {
				console.error(
					"Could not read from Data directory, trying direct path:",
					error
				);
				try {
					const file = await Filesystem.readFile({
						path: photo.filepath,
					});
					return {
						...photo,
						webviewPath: `data:image/jpeg;base64,${file.data}`,
					};
				} catch (fallbackError) {
					console.error("Direct path failed too:", fallbackError);
					if (isPlatform("hybrid")) {
						return {
							...photo,
							webviewPath: Capacitor.convertFileSrc(photo.filepath),
						};
					}
					throw fallbackError;
				}
			}
		}
		return photo;
	};

	const importPhoto = async (): Promise<UserPhoto> => {
		const photo = await Camera.getPhoto({
			resultType: CameraResultType.Uri,
			source: CameraSource.Photos,
			quality: 100,
		});

		const fileName = Date.now() + ".jpeg";
		return await savePicture(photo, fileName);
	};

	return {
		photos,
		takePhoto,
		takePhotoForEntry,
		deletePhoto,
		loadWebviewPath,
		importPhoto,
		savePicture,
	};
};
