export interface Note {
  id: string;
  text: string;
  timestamp: number;
  photos: string[];
}

export interface DailyEntry {
  date: string;
  notes: Note[];
  standalonePhotos: string[];
}

export interface UserPhoto {
  filepath: string;
  webviewPath?: string;
  timestamp?: number;
  associatedNoteId?: string;
}

export interface DailyEntriesData {
  entries: Record<string, DailyEntry>;
}