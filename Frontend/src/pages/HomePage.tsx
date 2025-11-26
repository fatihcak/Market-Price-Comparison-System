// src/pages/HomePage.tsx
import HoverSelecter from '../components/hover_selecter'; // Bileşeni içe aktar
import '../styles/hover_selecter_style.css'; // CSS dosyasını içe aktar

const HomePage = () => {
  return (
    <div>
      <HoverSelecter /> {/* Hover butonlarını buraya ekleyin */}
    </div>
  );
};

export default HomePage;