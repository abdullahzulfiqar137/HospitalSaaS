export interface Admission {
  id: string;
  admissionNumber: string;
  patientId: string;
  doctorId: string;
  admissionDateUtc: string;
  admissionType: string;
  reason: string;
  wardId: string;
  roomId: string;
  bedId: string;
  status: string;
  dischargeDateUtc: string | null;
}

export interface AdmissionHistory {
  id: string;
  admissionId: string;
  fromWardId: string;
  fromRoomId: string;
  fromBedId: string;
  toWardId: string;
  toRoomId: string;
  toBedId: string;
  reason: string | null;
  transferredAtUtc: string;
}

export interface AdmitPatientRequest {
  patientId: string;
  doctorId: string;
  admissionType: string;
  reason: string;
  wardId: string;
  roomId: string;
  bedId: string;
}

export interface TransferAdmissionRequest {
  wardId: string;
  roomId: string;
  bedId: string;
  reason: string | null;
}
