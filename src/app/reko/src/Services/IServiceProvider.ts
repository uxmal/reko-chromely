export interface IServiceProvider {
	requireService(serviceId: string): object
	getService(serviceId: string): object|null
}