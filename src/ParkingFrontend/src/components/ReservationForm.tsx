import { useState, useEffect } from 'react';
import { useUser } from '../context/UserContext';
import { useCreateReservation } from '../query/useCreateReservation';

interface ReservationFormProps {
  parkingSlotId: string;
  designation: string;
  onClose: () => void;
}

export const ReservationForm = ({ parkingSlotId, designation, onClose }: ReservationFormProps) => {
  // Initialize date to today using the YYYY-MM-DD format
  const [date, setDate] = useState(new Date().toISOString().split('T')[0]);
  const [startTime, setStartTime] = useState('');
  const [endTime, setEndTime] = useState('');
  const [validationError, setValidationError] = useState('');
  const [apiError, setApiError] = useState('');

  const { selectedUserId } = useUser();
  const createReservation = useCreateReservation();

  // Re-run validation whenever date, start time, or end time changes
  useEffect(() => {
    setApiError('');

    if (!date || !startTime || !endTime) {
      setValidationError('');
      return;
    }

    // Safely combine the separate date and time strings into valid Date objects
    const start = new Date(`${date}T${startTime}`);
    const end = new Date(`${date}T${endTime}`);
    const now = new Date();

    if (start < now) {
      setValidationError('Start time cannot be in the past.');
      return;
    }

    const diffInMinutes = (end.getTime() - start.getTime()) / (1000 * 60);

    if (diffInMinutes < 30) {
      setValidationError('End time must be at least 30 minutes after start time.');
      return;
    }

    setValidationError('');
  }, [date, startTime, endTime]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (validationError || !date || !startTime || !endTime) return;

    if (!selectedUserId) {
      setApiError('No user is currently selected. Please select one from the navigation bar.');
      return;
    }

    const start = new Date(`${date}T${startTime}`);
    const end = new Date(`${date}T${endTime}`);

    const request = {
      parkingSlotId,
      userId: selectedUserId,
      startTime: start.toISOString(),
      endTime: end.toISOString(),
    };

    createReservation.mutate(request, {
      onSuccess: () => {
        onClose();
      },
      onError: (error) => {
        const message = error.response?.data?.message || 'An unexpected error occurred.';
        setApiError(message);
      }
    });
  };

  const isFormValid = date && startTime && endTime && !validationError && !createReservation.isPending;

  return (
    <div>
      <h2 style={{ marginTop: 0, marginBottom: '0.5rem', color: '#111827' }}>Book Slot</h2>
      <p style={{ color: '#6b7280', marginBottom: '1.5rem' }}>
        Reserving slot: <strong>{designation}</strong>
      </p>

      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
        
        <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
          <label htmlFor="date" style={{ fontSize: '0.875rem', fontWeight: 600 }}>Date</label>
          <input
            type="date"
            id="date"
            value={date}
            onChange={(e) => setDate(e.target.value)}
            style={{ padding: '0.5rem', borderRadius: '4px', border: '1px solid #d1d5db' }}
            required
            disabled={createReservation.isPending}
          />
        </div>

        <div style={{ display: 'flex', gap: '1rem' }}>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem', flex: 1 }}>
            <label htmlFor="startTime" style={{ fontSize: '0.875rem', fontWeight: 600 }}>Start Time</label>
            <input
              type="time"
              id="startTime"
              value={startTime}
              onChange={(e) => setStartTime(e.target.value)}
              style={{ padding: '0.5rem', borderRadius: '4px', border: '1px solid #d1d5db', width: '100%', boxSizing: 'border-box' }}
              required
              disabled={createReservation.isPending}
            />
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem', flex: 1 }}>
            <label htmlFor="endTime" style={{ fontSize: '0.875rem', fontWeight: 600 }}>End Time</label>
            <input
              type="time"
              id="endTime"
              value={endTime}
              onChange={(e) => setEndTime(e.target.value)}
              style={{ padding: '0.5rem', borderRadius: '4px', border: '1px solid #d1d5db', width: '100%', boxSizing: 'border-box' }}
              required
              disabled={createReservation.isPending}
            />
          </div>
        </div>

        {/* Client-side Validation Warning */}
        {validationError && (
          <div style={{ color: '#ef4444', fontSize: '0.875rem', fontWeight: 500 }}>
            {validationError}
          </div>
        )}

        {/* Backend API Error Warning */}
        {apiError && (
          <div style={{ color: '#b91c1c', fontSize: '0.875rem', fontWeight: 500, backgroundColor: '#fee2e2', padding: '0.75rem', borderRadius: '4px' }}>
            {apiError}
          </div>
        )}

        <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '1rem', marginTop: '1rem' }}>
          <button
            type="button"
            onClick={onClose}
            disabled={createReservation.isPending}
            style={{
              padding: '0.5rem 1rem', borderRadius: '4px', border: '1px solid #d1d5db',
              backgroundColor: '#ffffff', cursor: createReservation.isPending ? 'not-allowed' : 'pointer'
            }}
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={!isFormValid}
            style={{
              padding: '0.5rem 1rem', borderRadius: '4px', border: 'none',
              backgroundColor: isFormValid ? '#3b82f6' : '#9ca3af',
              color: '#ffffff', cursor: isFormValid ? 'pointer' : 'not-allowed'
            }}
          >
            {createReservation.isPending ? 'Submitting...' : 'Submit'}
          </button>
        </div>
      </form>
    </div>
  );
};