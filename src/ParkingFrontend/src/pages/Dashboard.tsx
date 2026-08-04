import { useParkingSlots } from "../query/useParkingSlots"

export const Dashboard = () => {
    const {data: slots, isLoading, isError, error} = useParkingSlots();

    // Loading State
    if(isLoading) {
        return (
            <div style={{ textAlign: 'center', marginTop: '3rem', color: '#4b5563' }}>
                <h2>Loading parking slots...</h2>
            </div>
        )
    }

    // Error State
    if(isError) {
        return (
            <div style={{ textAlign: 'center', marginTop: '3rem', color: '#ef4444' }}>
              <h2>Failed to load data</h2>
              <p>{error?.message}</p>
            </div>
        );
    }

    // Success state
    return (
        <div>
          <h2 style={{ borderBottom: '2px solid #e5e7eb', paddingBottom: '0.5rem' }}>
            Parking Slots Availability
          </h2>

          <div style={{ 
            display: 'grid', 
            gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', 
            gap: '1.5rem',
            marginTop: '1.5rem'
          }}>
            {slots?.map((slot) => (
              <div key={slot.id} style={{
                border: '1px solid #d1d5db',
                borderRadius: '8px',
                padding: '1.5rem',
                backgroundColor: '#f9fafb',
                boxShadow: '0 2px 4px rgba(0,0,0,0.05)'
              }}>
                <h3 style={{ margin: '0 0 1rem 0', color: '#1f2937' }}>
                  Slot: {slot.designation}
                </h3>

                {slot.reservations.length === 0 ? (
                  <p style={{ color: '#6b7280', fontStyle: 'italic', margin: 0 }}>
                    Available / No reservations.
                  </p>
                ) : (
                  <ul style={{ 
                    listStyleType: 'none', 
                    padding: 0, 
                    margin: 0, 
                    display: 'flex', 
                    flexDirection: 'column', 
                    gap: '0.75rem' 
                  }}>
                    {slot.reservations.map((res) => {
                      const startLocal = new Date(res.startTime).toLocaleString();
                      const endLocal = new Date(res.endTime).toLocaleString();

                      return (
                        <li key={res.id} style={{
                          backgroundColor: '#ffffff',
                          border: '1px solid #e5e7eb',
                          padding: '0.75rem',
                          borderRadius: '6px',
                          fontSize: '0.875rem',
                          color: '#374151'
                        }}>
                          <div style={{ marginBottom: '4px' }}>
                            <span style={{ fontWeight: 600, color: '#10b981' }}>Start:</span> {startLocal}
                          </div>
                          <div>
                            <span style={{ fontWeight: 600, color: '#ef4444' }}>End:</span> {endLocal}
                          </div>
                        </li>
                      );
                    })}
                  </ul>
                )}
              </div>
            ))}
          </div>
        </div>
    );
}