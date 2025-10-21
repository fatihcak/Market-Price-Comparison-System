// src/router/index.tsx

import { createBrowserRouter } from 'react-router-dom';
import HomePage from '../pages/HomePage'; // HomePage'i import et

const routes = [
  {
    path: '/',
    element: <HomePage />, // '/' adresine gidilince HomePage'i göster
  },
];

const router = createBrowserRouter(routes);

export default router;