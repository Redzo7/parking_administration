import { useState } from 'react';
import { useParkingSlots } from '../query/useParkingSlots';
import { Modal } from '../components/Modal';
import { ReservationForm } from '../components/ReservationForm';
import { useUser } from '../context/UserContext';
import { useCancelReservation } from '../query/useCancelReservation';

export const Dashboard = () => {
  const { data: slots, isLoading, isError, error } = useParkingSlots();
  const { selectedUserId } = useUser();
  const cancelReservation = useCancelReservation();
  
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedSlot, setSelectedSlot] = useState<{ id: string; designation: string } | null>(null);

  if (isLoading) return <div style={{ textAlign: 'center', marginTop: '3rem', color: '#4b5563' }}><h2>Loading parking slots...</h2></div>;
  if (isError) return <div style={{ textAlign: 'center', marginTop: '3rem', color: '#ef4444' }}><h2>Failed to load data</h2><p>{error?.message}</p></div>;

  const handleOpenModal = (id: string, designation: string) => {
    setSelectedSlot({ id, designation });
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setSelectedSlot(null);
  };

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
        Parking Slots Availability
      </h2>
      
      <div style={{ 
        display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))', 
        gap: '1.5rem', marginTop: '1.5rem'
      }}>
        {slots?.map((slot) => (
          <div key={slot.id} style={{
            border: '1px solid #d1d5db', borderRadius: '8px', padding: '1.5rem',
            backgroundColor: '#f9fafb', boxShadow: '0 2px 4px rgba(0,0,0,0.05)'
          }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
              <h3 style={{ margin: 0, color: '#1f2937' }}>Slot: {slot.designation}</h3>
              <button 
                onClick={() => handleOpenModal(slot.id, slot.designation)}
                style={{
                  padding: '0.5rem 1rem', backgroundColor: '#10b981', color: 'white', 
                  border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 600, fontSize: '0.875rem'
                }}
              >
                Book Slot
              </button>
            </div>
            
            {slot.reservations.length === 0 ? (
              <p style={{ color: '#6b7280', fontStyle: 'italic', margin: 0 }}>Available / No reservations.</p>
            ) : (
              <ul style={{ listStyleType: 'none', padding: 0, margin: 0, display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
                {slot.reservations.map((res) => {
                  const startLocal = new Date(res.startTime).toLocaleString();
                  const endLocal = new Date(res.endTime).toLocaleString();
                  
                  // Security & UI Logic
                  const isFuture = new Date(res.startTime) > new Date();
                  const isOwner = res.userId === selectedUserId;
                  const canCancel = isFuture && isOwner;
                  
                  // Track loading state for this specific button
                  const isCancelingThis = cancelReservation.isPending && cancelReservation.variables === res.id;

                  return (
                    <li key={res.id} style={{
                      backgroundColor: '#ffffff', border: '1px solid #e5e7eb', padding: '0.75rem',
                      borderRadius: '6px', fontSize: '0.875rem', color: '#374151',
                      display: 'flex', justifyContent: 'space-between', alignItems: 'center'
                    }}>
                      <div>
                        <div style={{ marginBottom: '4px' }}><span style={{ fontWeight: 600, color: '#10b981' }}>Start:</span> {startLocal}</div>
                        <div><span style={{ fontWeight: 600, color: '#ef4444' }}>End:</span> {endLocal}</div>
                      </div>
                      
                      {/* Conditionally Render Cancel Button */}
                      {canCancel && (
                        <button
                          onClick={() => handleCancel(res.id)}
                          disabled={isCancelingThis}
                          style={{
                            padding: '0.4rem 0.75rem',
                            backgroundColor: isCancelingThis ? '#f3f4f6' : '#fee2e2',
                            color: isCancelingThis ? '#9ca3af' : '#b91c1c',
                            border: `1px solid ${isCancelingThis ? '#d1d5db' : '#fca5a5'}`,
                            borderRadius: '4px',
                            cursor: isCancelingThis ? 'not-allowed' : 'pointer',
                            fontWeight: 600,
                            fontSize: '0.75rem',
                            transition: 'background-color 0.2s'
                          }}
                        >
                          {isCancelingThis ? 'Canceling...' : 'Cancel'}
                        </button>
                      )}
                    </li>
                  );
                })}
              </ul>
            )}
          </div>
        ))}
      </div>

      <Modal isOpen={isModalOpen} onClose={handleCloseModal}>
        {selectedSlot && (
          <ReservationForm 
            parkingSlotId={selectedSlot.id} 
            designation={selectedSlot.designation} 
            onClose={handleCloseModal} 
          />
        )}
      </Modal>
    </div>
  );
};