export default function Unauthorized() {
  return (
    <div style={{ textAlign: 'center', marginTop: '50px' }}>
      <h1>403 - Tsy mahazo miditra</h1>
      <p>Miala tsiny, fa tsy manana alalana hiditra amin'ity pejy ity ianao.</p>
      <button onClick={() => window.history.back()}>Miverina</button>
    </div>
  );
}