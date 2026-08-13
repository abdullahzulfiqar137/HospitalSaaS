export interface CreateSubscriptionRequest {
  plan: string;
  startsAtUtc: string;
  endsAtUtc: string;
}

export interface Subscription extends CreateSubscriptionRequest {
  id: string;
  status: string;
}
