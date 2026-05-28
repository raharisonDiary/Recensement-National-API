import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip, Legend } from 'recharts';

export default function Statistique() {
  const data = [
    { name: '5-10 taona', value: 400 },
    { name: '18-30 taona', value: 300 },
    { name: '30-60 taona', value: 200 }
  ];
  const COLORS = ['#0088FE', '#00C49F', '#FFBB28'];

  return (
    <div style={{ height: 400, width: '100%' }}>
      <h2>Statistiques Globale</h2>
      <ResponsiveContainer>
        <PieChart>
          <Pie data={data} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={80} label>
            {data.map((entry, index) => <Cell key={index} fill={COLORS[index % COLORS.length]} />)}
          </Pie>
          <Tooltip />
          <Legend />
        </PieChart>
      </ResponsiveContainer>
    </div>
  );
}