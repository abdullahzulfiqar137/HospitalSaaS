export type AppointmentStatus =
  'Scheduled' | 'Confirmed' | 'CheckedIn' | 'InProgress' | 'Completed' | 'Cancelled' | 'NoShow';
export type AppointmentAction =
  'Confirm' | 'CheckIn' | 'Start' | 'Complete' | 'Cancel' | 'NoShow' | 'Reschedule';

export interface Appointment {
  id: string;
  patientId: string;
  doctorId: string;
  departmentId: string;
  appointmentDateTimeUtc: string;
  durationMinutes: number;
  appointmentType: string;
  status: AppointmentStatus;
  reason: string;
  notes: string | null;
}

export interface BookAppointmentRequest {
  patientId: string;
  doctorId: string;
  departmentId: string;
  appointmentDateTimeUtc: string;
  durationMinutes: number;
  appointmentType: string;
  reason: string;
  notes: string | null;
}

export type UpdateAppointmentRequest = Omit<BookAppointmentRequest, 'patientId'>;

export interface AppointmentActionRequest {
  action: AppointmentAction;
  reason: string | null;
  appointmentDateTimeUtc: string | null;
  durationMinutes: number | null;
}

export interface AppointmentQuery {
  date?: string;
  doctorId?: string;
  patientId?: string;
  status?: AppointmentStatus;
  pageNumber?: number;
  pageSize?: number;
}
