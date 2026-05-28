import { useState, useEffect } from 'react';
import axiosClient from '../api/axiosClient';
import { QRCodeCanvas } from 'qrcode.react';

export default function GestionRegionaux() {
  const [regionaux, setRegionaux] = useState([]);
  const [selectedRegional, setSelectedRegional] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const [reg, setReg] = useState({ nom: '', cin: '', password: '' });

  // Namboarina: Natao ao anatin'ny useEffect ny fetch ary 
  // nampiana "isMounted" mba hisorohana ny warning rehetra
  useEffect(() => {
    let isMounted = true;

    const loadData = async () => {
      try {
        const { data } = await axiosClient.get('/Users/regionaux');
        if (isMounted) {
          setRegionaux(data);
        }
      } catch (err) {
        console.error("Error fetching data:", err);
      }
    };

    loadData();

    return () => {
      isMounted = false; // Manadio ny state raha toa ka misy component unmount
    };
  }, []);

  // Ireo functions hafa dia mijanona toy ny teo aloha
  const refreshList = async () => {
    try {
      const { data } = await axiosClient.get('/Users/regionaux');
      setRegionaux(data);
    } catch (err) {
      console.error(err);
    }
  };

  const addRegional = async () => {
    if (!reg.nom || !reg.cin || !reg.password) return alert("Fenoy ny saha rehetra!");
    try {
      await axiosClient.post('/Users/create-regional', reg);
      setReg({ nom: '', cin: '', password: '' });
      refreshList();
    } catch (err) { 
      console.error(err); 
      alert("Nisy olana teo amin'ny fampidirana."); 
    }
  };

  const handleUpdate = async () => {
    try {
      await axiosClient.put(`/Users/update-regional/${selectedRegional.id}`, selectedRegional);
      setIsEditing(false);
      setSelectedRegional(null);
      refreshList();
    } catch (err) { 
      console.error(err); 
      alert("Tsy tafaverina ny fanovana."); 
    }
  };

  const deleteRegional = async (id) => {
    if (!window.confirm("Hofafana ve ity Régional ity?")) return;
    try {
      await axiosClient.delete(`/Users/delete-regional/${id}`);
      refreshList();
    } catch (err) { 
      console.error(err); 
      alert("Tsy tafafafa"); 
    }
  };

  return (
    <div style={{ padding: '20px' }}>
      <h2>Gestion Régionaux</h2>

      <div style={{ marginBottom: '20px', display: 'flex', gap: '10px' }}>
        <input placeholder="Anarana" value={reg.nom} onChange={e => setReg({...reg, nom: e.target.value})} />
        <input placeholder="CIN" value={reg.cin} onChange={e => setReg({...reg, cin: e.target.value})} />
        <input type="password" placeholder="Password" value={reg.password} onChange={e => setReg({...reg, password: e.target.value})} />
        <button onClick={addRegional}>Ampio</button>
      </div>

      <table border="1" style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr><th>Anarana</th><th>CIN</th><th>Actions</th></tr>
        </thead>
        <tbody>
          {regionaux.map(r => (
            <tr key={r.id}>
              <td>{r.nom}</td>
              <td>{r.cin}</td>
              <td style={{ display: 'flex', gap: '5px' }}>
                <button onClick={() => { setSelectedRegional(r); setIsEditing(false); }}>Details</button>
                <button onClick={() => { setSelectedRegional(r); setIsEditing(true); }}>Edit</button>
                <button onClick={() => deleteRegional(r.id)} style={{ color: 'red' }}>Fafana</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {selectedRegional && (
        <div style={{ position: 'fixed', top: 0, left: 0, width: '100%', height: '100%', background: 'rgba(0,0,0,0.5)', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
          <div style={{ background: 'white', padding: '20px', borderRadius: '8px', width: '400px' }}>
            {isEditing ? (
              <>
                <h3>Edit {selectedRegional.nom}</h3>
                <input value={selectedRegional.nom} onChange={e => setSelectedRegional({...selectedRegional, nom: e.target.value})} />
                <div style={{ marginTop: '10px' }}>
                  <button onClick={handleUpdate}>Tehirizo</button>
                  <button onClick={() => { setSelectedRegional(null); setIsEditing(false); }}>Ajanona</button>
                </div>
              </>
            ) : (
              <>
                <h3>Mombamomba</h3>
                <p>Anarana: {selectedRegional.nom}</p>
                <p>CIN: {selectedRegional.cin}</p>
                <QRCodeCanvas value={selectedRegional.id.toString()} size={80} />
                <div style={{ marginTop: '15px' }}>
                  <button onClick={() => window.print()}>Print</button>
                  <button onClick={() => setSelectedRegional(null)}>Akato</button>
                </div>
              </>
            )}
          </div>
        </div>
      )}
    </div>
  );
}