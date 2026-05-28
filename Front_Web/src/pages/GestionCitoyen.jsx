import { useState } from 'react';
import axiosClient from '../api/axiosClient';

export default function GestionCitoyen() {
  const initialFormState = {
    Nom: '',
    DateNaissance: '',
    Sexe: 'M',
    EstMarie: 'false',
    NbEnfants: 0,
    NoCin: '',
    MenageId: localStorage.getItem('currentMenageId') || ''
  };

  const [data, setData] = useState(initialFormState);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!data.MenageId) {
      alert("Tsy maintsy misafidy na mamorona Ménage aloha!");
      return;
    }

    setLoading(true);
    
    // Fikarakarana ny data ho an'ny fitehirizana
    const newCitoyen = {
      ...data,
      Id: crypto.randomUUID(), // ID Unique ho an'ny sync
      EstMarie: data.EstMarie === 'true',
      NbEnfants: parseInt(data.NbEnfants) || 0
    };

    try {
      // 1. Miezaka mandefa mivantana any amin'ny API (Online)
      await axiosClient.post('/Citoyens', newCitoyen);
      alert("Voatahiry soa aman-tsara ny Citoyen (Online)!");
    } catch (err) {
      // 2. Raha misy error (Offline), tehirizo anaty localStorage
      console.warn("Offline detected, saving locally...", err);
      const pendingData = JSON.parse(localStorage.getItem('pending_citoyens') || '[]');
      pendingData.push(newCitoyen);
      localStorage.setItem('pending_citoyens', JSON.stringify(pendingData));
      
      alert("Tsy tafakatra ny data fa voatahiry an-toerana (Offline). Aza adino ny manao SYNC rehefa misy Internet!");
    } finally {
      setData(initialFormState); // Reset ny form
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '15px', maxWidth: '400px', margin: '20px' }}>
      <h2>Enregistrement Citoyen</h2>
      
      <input placeholder="Anarana Feno" value={data.Nom} required onChange={e => setData({...data, Nom: e.target.value})} />
      
      <input type="text" placeholder="Laharana CIN" value={data.NoCin} onChange={e => setData({...data, NoCin: e.target.value})} />
      
      <label>Daty nahaterahana:</label>
      <input type="date" value={data.DateNaissance} required onChange={e => setData({...data, DateNaissance: e.target.value})} />
      
      <label>Lahy sa Vavy?</label>
      <select value={data.Sexe} onChange={e => setData({...data, Sexe: e.target.value})}>
        <option value="M">Lahy</option>
        <option value="F">Vavy</option>
      </select>

      <label>Manambady ve?</label>
      <select value={data.EstMarie} onChange={e => setData({...data, EstMarie: e.target.value})}>
        <option value="false">Tsy manambady</option>
        <option value="true">Manambady</option>
      </select>

      {data.EstMarie === 'true' && (
        <input type="number" placeholder="Isan'ny zanaka" value={data.NbEnfants} onChange={e => setData({...data, NbEnfants: e.target.value})} />
      )}

      <p style={{ fontSize: '0.8em', color: '#555' }}>
        <strong>Ménage ID:</strong> {data.MenageId || "Tsy mbola misy"}
      </p>
      
      <button type="submit" disabled={loading} style={{ padding: '10px', cursor: 'pointer' }}>
        {loading ? "Mitehirizana..." : "Hatahiry"}
      </button>
    </form>
  );
}