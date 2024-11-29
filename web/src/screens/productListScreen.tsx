import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { ApiService } from '../services/apiService';
import '../App.css';

interface Product {
    id: number;
    name: string;
    category: string;
    price: Number;
}

const ProductListScreen: React.FC = () => {
    const [Products, setProducts] = useState<Product[]>([]);
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const ProductEmptyRow: Product = { id: 0, name: '', category: '', price: 0 };
    let ProductsWithEmptyRow: Product[] = [];

    useEffect(() => {
        fetchProducts();
    }, []);



    const fetchProducts = async () => {
        setIsLoading(true);
        setError('');
        try {
            const data = await ApiService.get<Product[]>('Products');
            setProducts(data);
        } catch (err: any) {
            setError(err.message || 'Erro ao buscar funcionários');
        } finally {
            setIsLoading(false);
        }
    };

    const saveProduct = async (Product: Product) => {
        setIsLoading(true);
        const isNew = Product.id == 0 || Product.id == undefined;
        try {
            const endpoint = isNew ? 'Products' : `Products/${Product.id}`;
            await ApiService[isNew ? 'post' : 'put'](endpoint, Product);
            fetchProducts();
        } catch (err: any) {
            setError(err.message || 'Erro ao salvar produto');
        } finally {
            setIsLoading(false);
        }
    };

    const deleteProduct = async (id: number) => {
        setIsLoading(true);
        try {
            await ApiService.delete(`Products/${id}`);
            fetchProducts();
        } catch (err: any) {
            setError(err.message || 'Erro ao deletar produto');
        } finally {
            setIsLoading(false);
        }
    };

    const updateField = (index: number, field: keyof Product, value: string) => {
        const updatedProducts = [...Products];
        updatedProducts[index] = { ...updatedProducts[index], [field]: value };
        setProducts(updatedProducts);
    };

    const addEmptyRow = () => {
        ProductsWithEmptyRow.push(ProductEmptyRow);
    }

    ProductsWithEmptyRow = [...Products];
    if (!Products.some(obj => obj.id === ProductEmptyRow.id && obj.name === ProductEmptyRow.name && obj.category === ProductEmptyRow.category && obj.price === ProductEmptyRow.price) &&
        Products.every((obj) => 'id' in obj && obj.id > 0))
        addEmptyRow();

    const handleLogout = () => {
        localStorage.removeItem('authToken');
        window.location.href = '/';
    };

    return (
        <div className="Product-container">
            <button
                className="logout-button"
                onClick={handleLogout}
            >
                Sair
            </button>
            {isLoading && (
                <div className="loading-overlay">
                    <div className="spinner"></div>
                </div>
            )}
            <h1>Gerenciamento de produto</h1>

            {error && <p className="error">{error}</p>}
            <table>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Produto</th>
                        <th>Categoria</th>
                        <th>Preçoo</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {ProductsWithEmptyRow.map((Product, index) => (
                        <tr key={index}>
                            <td>{Product.id || '-'}</td>
                            <td>
                                <input
                                    type="text"
                                    value={Product.name}
                                    onChange={(e) => updateField(index, 'name', e.target.value)}
                                />
                            </td>
                            <td>
                                <input
                                    type="text"
                                    value={Product.category}
                                    onChange={(e) => updateField(index, 'category', e.target.value)}
                                />
                            </td>
                            <td>
                                <input
                                    type="number"
                                    value={Product.price}
                                    onChange={(e) => updateField(index, 'price', e.target.value)}
                                />
                            </td>
                            <td className="actions-btns">
                                {Product.id == 0 || Product.id == undefined ? (
                                    <button className="add-btn" onClick={() => saveProduct(Product)}>
                                        Adicionar
                                    </button>
                                ) : (
                                    <>
                                        <button className="edit-btn" onClick={() => saveProduct(Product)}>
                                            Editar
                                        </button>
                                        <button className="delete-btn" onClick={() => deleteProduct(Product.id)}>
                                            Apagar
                                        </button>
                                    </>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default ProductListScreen;
