import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConfirmationDialogComponent } from './confirmation-dialog.component';

describe('ConfirmationDialogComponent', () => {
  let fixture: ComponentFixture<ConfirmationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfirmationDialogComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(ConfirmationDialogComponent);
    fixture.componentRef.setInput('title', 'Delete record?');
    fixture.componentRef.setInput('message', 'This cannot be undone.');
    fixture.detectChanges();
  });

  it('renders the supplied confirmation copy', () => {
    expect(fixture.nativeElement.textContent).toContain('Delete record?');
    expect(fixture.nativeElement.textContent).toContain('This cannot be undone.');
  });

  it('emits confirmed from the destructive button', () => {
    const confirmed = jasmine.createSpy('confirmed');
    fixture.componentInstance.confirmed.subscribe(confirmed);
    fixture.nativeElement.querySelector('.confirm').click();
    expect(confirmed).toHaveBeenCalled();
  });
});
