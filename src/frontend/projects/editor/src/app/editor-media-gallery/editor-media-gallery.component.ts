import { Component, inject, OnInit } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { AsyncPipe } from '@angular/common';
import { BehaviorSubject, debounceTime, distinctUntilChanged } from 'rxjs';
import { BaseComponent, SAMediaInfo, SAMediaInfoSAMediaInfo, SAMediaSearchRequest, SchulaufgabenEditorWebV1Service, SelectionService } from 'schulaufgaben';

@Component({
  selector: 'app-editor-media-gallery',
  imports: [
    AsyncPipe,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './editor-media-gallery.component.html',
  styleUrl: './editor-media-gallery.component.scss',
})
export class EditorMediaGalleryComponent extends BaseComponent implements OnInit {
  readonly client = inject(SchulaufgabenEditorWebV1Service);
  readonly selectionService = inject(SelectionService);

  readonly searchControl = new FormControl('');
  readonly searchResults$ = new BehaviorSubject<SAMediaInfo[] | null>(null);
  readonly isLoading$ = new BehaviorSubject<boolean>(false);

  constructor() {
    super();
  }

  ngOnInit(): void {
    this.selectionService.title$.next('Media Gallery');

    // Subscribe to search input changes with debounce
    this.subscriptions.add(
      this.searchControl.valueChanges
        .pipe(
          debounceTime(300),
          distinctUntilChanged()
        )
        .subscribe(value => {
          if (value) {
            this.search(value);
          }
        })
    );
  }

  search(query: string): void {
    this.isLoading$.next(true);
    const mediaSearchRequest: SAMediaSearchRequest = {
      mediaType: 1, //MediaType.Image,
      value: query
    };
    this.client.postAPIMediaSearch(mediaSearchRequest).subscribe({
      next: (results) => {
        this.searchResults$.next(results);
        this.isLoading$.next(false);
      },
      error: (error) => {
        console.error('Error searching media:', error);
        this.isLoading$.next(false);
      }
    });
  }

  onSearchClick(): void {
    const query = this.searchControl.value || '';
    this.search(query);
  }

  getMediaThumbnailUrl(mediaInfo: SAMediaInfo): string {
    return `/API/Media/Thumbnail/${mediaInfo.path}`;
  }
  getMediaContentUrl(mediaInfo: SAMediaInfo): string {
    return `/API/Media/Content/${mediaInfo.path}`;
  }

  getMediaFileName(mediaInfo: SAMediaInfo) {
    const parts = (mediaInfo.path ?? '').split('/');
    return parts.length === 0 ? mediaInfo.path : parts[parts.length - 1];
  }

  isImage(mediaInfo: SAMediaInfo): boolean {
    return mediaInfo.mediaType === 'Image';
    // const ext = path.toLowerCase().split('.').pop();
    // return ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'].includes(ext || '');
  }

  isVideo(mediaInfo: SAMediaInfo): boolean {
    return mediaInfo.mediaType === 'Video';
    // const ext = path.toLowerCase().split('.').pop();
    // return ['mp4', 'webm', 'ogg', 'mov', 'avi'].includes(ext || '');
  }

  isAudio(mediaInfo: SAMediaInfo): boolean {
    return mediaInfo.mediaType === 'Audio';
    // const ext = path.toLowerCase().split('.').pop();
    // return ['mp3', 'wav', 'ogg', 'aac', 'm4a'].includes(ext || '');
  }

  /*
  <p class="media-size">{{ formatSize(item.size) }}</p>
  formatSize(size: number | string | undefined): string {
    if (size === undefined) return 'Unknown';
    const bytes = typeof size === 'string' ? parseInt(size, 10) : size;
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(2)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(2)} MB`;
  }
  */
}
