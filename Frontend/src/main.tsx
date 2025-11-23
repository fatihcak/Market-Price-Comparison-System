// src/main.tsx

import React from 'react';
import ReactDOM from 'react-dom/client';
import { RouterProvider } from 'react-router-dom'; // BU SATIR ÖNEMLİ
import router from './router'; // BU SATIR ÖNEMLİ (router/index.tsx dosyamız)
import './index.css'; 

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    {/* BURASI ÇOK ÖNEMLİ!
      Eski <App /> kodunu sildik, yerine bunu yazdık:
    */}
    <RouterProvider router={router} />
  </React.StrictMode>,
);