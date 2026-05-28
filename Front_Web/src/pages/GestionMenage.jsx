import { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import axiosClient from '../api/axiosClient';

export default function GestionMenage() {
  const { user } = useAuth();
  const [formData, setFormData] = useState({ 
    Region: '', District: '', Fokontany: '', GpsLat: 0, GpsLong: 0 
  });

  const getGPS = () => {
    if (!navigator.geolocation) return alert("Tsy mandeha ny GPS");
    navigator.geolocation.getCurrentPosition((pos) => {
      setFormData(prev => ({ 
        ...prev, 
        GpsLat: pos.coords.latitude, 
        GpsLong: pos.coords.longitude 
      }));
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const dataToSend = { ...formData, AgentId: user?.id, Id: crypto.randomUUID() };

    try {
      const response = await axiosClient.post('/Menages', dataToSend);
      localStorage.setItem('currentMenageId', response.data.id);
      alert("Voatahiry! Afaka manampy Citoyen ianao izao.");
    } catch (err) {
      console.error("Sync failed, saving offline:", err); // Efa tsy hisy error intsony ny eslint
      localStorage.setItem('currentMenageId', dataToSend.Id); 
      const pending = JSON.parse(localStorage.getItem('pending_menages') || '[]');
      pending.push(dataToSend);
      localStorage.setItem('pending_menages', JSON.stringify(pending));
      
      alert("Offline: Voatahiry an-toerana ny Ménage.");
    }
  };

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '15px', maxWidth: '400px', margin: '20px' }}>
      <h2>Enregistrement Ménage</h2>
      <input type="text" placeholder="Faritra" required onChange={e => setFormData({...formData, Region: e.target.value})} />
      <input type="text" placeholder="District" required onChange={e => setFormData({...formData, District: e.target.value})} />
      <input type="text" placeholder="Fokontany" required onChange={e => setFormData({...formData, Fokontany: e.target.value})} />
      <button type="button" onClick={getGPS}>Maka GPS</button>
      <p style={{ fontSize: '0.8em' }}>Lat: {formData.GpsLat}, Long: {formData.GpsLong}</p>
      <button type="submit">Hatahiry Ménage</button>
    </form>
  );
}