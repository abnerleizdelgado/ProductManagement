import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import App from './App';
import Register from './screens/registerScreen';
import Products from './screens/productListScreen';
import PrivateRoute from './services/privateRouteService';

function Main() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<App />} />
                <Route path="/register" element={<Register />} />
                <Route
                    path="/products"
                    element={<PrivateRoute element={<Products />} />}
                />
            </Routes>
        </Router>
    );
}

ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
        <Main />
    </React.StrictMode>
);
