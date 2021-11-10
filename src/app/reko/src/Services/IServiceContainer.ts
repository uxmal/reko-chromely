export interface IServiceContainer {
	removeService<T extends object>(serviceId: string): boolean
	addService<T extends object>(serviceId: string, serviceInstance: T): void
}