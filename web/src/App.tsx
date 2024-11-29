import { useState } from 'react';
import { ApiService } from './services/apiService';
import './App.css';

function App() {
    const [userName, setUserName] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string>('');
    const [isLoading, setIsLoading] = useState<boolean>(false);

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsLoading(true);
        setError('');

        try {
            const { token } = await ApiService.post<{ token: string }>('Users/login', {
                userName,
                password,
            });

            localStorage.setItem('token', token);
            window.location.href = '/products';
        } catch (error: any) {
            if (error.message.includes('401')) {
                setError('Você ainda não possui um login, Cadastre-se!');
            } else {
                setError(error.message || 'Erro ao fazer login. Tente novamente.');
            }

            setTimeout(() => setError(''), 5000);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="login-container">
            <h1>Login</h1>
            <form onSubmit={handleLogin}>
                <div className="form-group">
                    <label htmlFor="user">Usuário:</label>
                    <input
                        type="text"
                        id="user"
                        value={userName}
                        onChange={(e) => setUserName(e.target.value)}
                        required
                        placeholder="Digite seu usuário"
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="password">Senha:</label>
                    <input
                        type="password"
                        id="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        placeholder="Digite sua senha"
                    />
                </div>
                <button type="submit" disabled={isLoading}>
                    {isLoading ? 'Entrando...' : 'Entrar'}
                </button>
            </form>
            <p className="register-link">
                Ainda n�o tem uma conta? <a href="/register">Cadastre-se aqui</a>
            </p>
            {error && <p className="error">{error}</p>}
        </div>
    );
}

export default App;
