import type { RootState } from '../../../app/store';
import type { ServiceKey } from './types';

export const selectHomeServiceHealth = (state: RootState) => state.homeServiceHealth;

export const selectServiceHealth = (service: ServiceKey) => (state: RootState) =>
  state.homeServiceHealth[service];
