import { inject, TestBed } from '@angular/core/testing';

import { DrivePickerService } from './drive-picker.service';

describe('DrivePickerService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DrivePickerService]
    });
  });

  it('should be created', inject([DrivePickerService], (service: DrivePickerService) => {
    expect(service).toBeTruthy();
  }));
});
