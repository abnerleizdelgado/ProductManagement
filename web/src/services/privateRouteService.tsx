import React from 'react';
import { Navigate } from 'react-router-dom';
import { jwtDecode } from "jwt-decode";

const PrivateRouteService = ({ element }) => {
    if (!isAuthenticated()) {
        return <Navigate to="/" replace />;
    }

    return element;
};

const isAuthenticated = () => {
    const token = localStorage.getItem('token');

    if (!token) return false;

    try {
        const decodedToken = jwtDecode(token);

        const currentTime = Date.now() / 1000;
        if (decodedToken.exp < currentTime) {
            localStorage.removeItem('token');
            return false;
        }
        return true;
    } catch (error) {
        console.error("Erro ao decodificar o token:", error);
        localStorage.removeItem('token');
        return false;
    }
};

export default PrivateRouteService;
