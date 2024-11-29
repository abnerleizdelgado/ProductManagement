import { useState } from 'react';
import { ApiService } from '../services/apiService';
import '../App.css';

function RegisterScreen() {
    const [userName, setUserName] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState<string>('');
    const [isLoading, setIsLoading] = useState<boolean>(false);

    const handleRegister = async (e: React.FormEvent) => {
        e.preventDefault();

        if (password !== confirmPassword) {
            setError('As senhas não coincidem!');
            return;
        }

        setIsLoading(true);
        setError('');

        try {
            const data = await ApiService.post('Users/create', { userName, password });
            window.location.href = '/';
        } catch (error: any) {
            setError(error.message || 'Erro ao realizar o cadastro. Tente novamente.');
            console.error('Erro no cadastro:', error);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="login-container">
            <button
                className="back-button"
                onClick={() => window.history.back()}
            >
                Voltar
            </button>
            <h1>Cadastrar</h1>
            <form onSubmit={handleRegister}>
                <div className="form-group">
                    <label htmlFor="user">Usuário:</label>
                    <input
                        type="text"
                        id="user"
                        value={userName}
                        onChange={(e) => setUserName(e.target.value)}
                        required
                        placeholder="Escolha um nome de usuário"
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
                <div className="form-group">
                    <label htmlFor="confirmPassword">Confirmar Senha:</label>
                    <input
                        type="password"
                        id="confirmPassword"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
                        required
                        placeholder="Confirme sua senha"
                    />
                </div>
                <button type="submit" disabled={isLoading}>
                    {isLoading ? 'Cadastrando...' : 'Cadastrar'}
                </button>
            </form>
            {error && <p className="error">{error}</p>}
        </div>
    );
}

export default RegisterScreen;
