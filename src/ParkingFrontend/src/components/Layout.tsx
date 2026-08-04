import { Outlet, Link } from 'react-router-dom';
import { useEffect } from 'react';
import { useUsers } from '../query/useUsers';
import { useUser } from '../context/UserContext';

export const Layout = () => {
  const { data: users, isLoading, isError } = useUsers();
  const { selectedUserId, setSelectedUserId } = useUser();

  // Set the default selected user to the first user in the fetched list
  useEffect(() => {
    if (users && users.length > 0 && !selectedUserId) {
      setSelectedUserId(users[0].id);
    }
  }, [users, selectedUserId, setSelectedUserId]);

  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <nav style={{ 
        display: 'flex', 
        justifyContent: 'space-between', 
        alignItems: 'center', 
        padding: '1rem 2rem', 
        backgroundColor: '#f3f4f6', 
        borderBottom: '1px solid #e5e7eb' 
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '2rem' }}>
          <h2 style={{ margin: 0, color: '#111827' }}>Parking Admin</h2>
          <Link to="/" style={{ textDecoration: 'none', color: '#374151', fontWeight: 'bold' }}>
            Dashboard
          </Link>
        </div>

        {/* User Selection Dropdown */}
        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
          <label htmlFor="user-select" style={{ fontSize: '0.875rem', fontWeight: 600, color: '#374151' }}>
            Simulated User:
          </label>
          
          {isLoading ? (
            <span style={{ fontSize: '0.875rem', color: '#6b7280' }}>Loading users...</span>
          ) : isError ? (
            <span style={{ fontSize: '0.875rem', color: '#ef4444' }}>Failed to load users</span>
          ) : (
            <select
              id="user-select"
              value={selectedUserId}
              onChange={(e) => setSelectedUserId(e.target.value)}
              style={{
                padding: '0.5rem',
                borderRadius: '6px',
                border: '1px solid #d1d5db',
                backgroundColor: '#ffffff',
                color: '#111827',
                cursor: 'pointer'
              }}
            >
              {users?.map((user) => (
                <option key={user.id} value={user.id}>
                  {user.name}
                </option>
              ))}
            </select>
          )}
        </div>
      </nav>

      <main style={{ padding: '2rem', flex: 1, backgroundColor: '#ffffff' }}>
        <Outlet />
      </main>
    </div>
  );
};