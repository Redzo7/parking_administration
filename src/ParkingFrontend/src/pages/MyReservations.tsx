import { useUser } from '../context/UserContext';
import { useUserReservations } from '../query/useUserReservations';
import { useCancelReservation } from '../query/useCancelReservation';

export const MyReservations = () => {
  const { selectedUserId } = useUser();
  const { data: reservations, isLoading, isError, error } = useUserReservations(selectedUserId);
  const cancelReservation = useCancelReservation();

  if (!selectedUserId) {
    return <div style={{ textAlign: 'center', marginTop: '3rem' }}>Please select a user to view their reservations.</div>;
  }

  if (isLoading) return <div style={{ textAlign: 'center', marginTop: '3rem', color: '#4b5563' }}><h2>Loading your reservations...</h2></div>;
  if (isError) return <div style={{ textAlign: 'center', marginTop: '3rem', color: '#ef4444' }}><h2>Failed to load data</h2><p>{error?.message}</p></div>;

  const handleCancel = (reservationId: string) => {
    if (window.confirm('Are you sure you want to cancel this reservation?')) {
      cancelReservation.mutate(reservationId, {
        onError: (err) => {
          alert(err.response?.data?.message || 'Failed to cancel reservation');
        }
      });
    }
  };

  return (
    <div>
      <h2 style={{ borderBottom: '2px solid #e5e7eb', paddingBottom: '0.5rem' }}>
        My Reservations
      </h2>
      
      {reservations?.length === 0 ? (
        <p style={{ color: '#6b7280', marginTop: '1.5rem' }}>You have no reservations.</p>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem', marginTop: '1.5rem' }}>
          {reservations?.map((res) => {
            const startLocal = new Date(res.startTime).toLocaleString();
            const endLocal = new Date(res.endTime).toLocaleString();
            
            // UI Logic: Only future reservations can be canceled
            const isFuture = new Date(res.startTime) > new Date();
            const isCancelingThis = cancelReservation.isPending && cancelReservation.variables === res.id;

            return (
              <div key={res.id} style={{
                display: 'flex', justifyContent: 'space-between', alignItems: 'center',
                border: '1px solid #d1d5db', borderRadius: '8px', padding: '1rem 1.5rem',
                backgroundColor: '#f9fafb', boxShadow: '0 1px 3px rgba(0,0,0,0.05)'
              }}>
                <div>
                  <h3 style={{ margin: '0 0 0.5rem 0', color: '#1f2937' }}>
                    Slot: {res.parkingSlotDesignation || res.parkingSlotId}
                  </h3>
                  <div style={{ fontSize: '0.875rem', color: '#374151' }}>
                    <div style={{ marginBottom: '4px' }}><span style={{ fontWeight: 600, color: '#10b981' }}>Start:</span> {startLocal}</div>
                    <div><span style={{ fontWeight: 600, color: '#ef4444' }}>End:</span> {endLocal}</div>
                  </div>
                </div>

                {isFuture && (
                  <button
                    onClick={() => handleCancel(res.id)}
                    disabled={isCancelingThis}
                    style={{
                      padding: '0.5rem 1rem',
                      backgroundColor: isCancelingThis ? '#f3f4f6' : '#fee2e2',
                      color: isCancelingThis ? '#9ca3af' : '#b91c1c',
                      border: `1px solid ${isCancelingThis ? '#d1d5db' : '#fca5a5'}`,
                      borderRadius: '4px',
                      cursor: isCancelingThis ? 'not-allowed' : 'pointer',
                      fontWeight: 600,
                      fontSize: '0.875rem',
                      transition: 'background-color 0.2s'
                    }}
                  >
                    {isCancelingThis ? 'Canceling...' : 'Cancel'}
                  </button>
                )}
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
};