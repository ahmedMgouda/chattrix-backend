export interface AppUser {
  id: string;
  name: string;
}

export const USERS: AppUser[] = [
  { id: '1', name: 'alice' },
  { id: '2', name: 'bob' },
  { id: '3', name: 'charlie' }
];
