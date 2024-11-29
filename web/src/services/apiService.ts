export class ApiService {
    private static baseUrl = 'http://localhost:5000/api/';

    static async request<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
        const token = localStorage.getItem('token');
        const headers = {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
            ...options.headers,
        };

        const response = await fetch(`${this.baseUrl}${endpoint}`, { ...options, headers });
        if (!response.ok) {
            const errorMessage = await response.clone().text();
            throw new Error(errorMessage || 'Erro na requisi��o');
        }
        let data: any = {};
        const contentType: any = response.headers.get("content-type");
        if (contentType && contentType.includes("application/json"))
            data = await response.json();

        return data;
    }

    static get<T>(endpoint: string): Promise<T> {
        return this.request<T>(endpoint);
    }

    static post<T>(endpoint: string, body: any): Promise<T> {
        return this.request<T>(endpoint, { method: 'POST', body: JSON.stringify(body) });
    }

    static put<T>(endpoint: string, body: any): Promise<T> {
        return this.request<T>(endpoint, { method: 'PUT', body: JSON.stringify(body) });
    }

    static delete<T>(endpoint: string): Promise<T> {
        return this.request<T>(endpoint, { method: 'DELETE' });
    }
}
