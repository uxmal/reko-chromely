export interface IServiceContainer {
	addService(serviceId: string, serviceInstance: object): void
}