import { useState } from "react";

interface DeleteModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
}

const DeleteModal: React.FC<DeleteModalProps> = ({
  isOpen,
  onClose,
  onConfirm,
}) => {
  if (!isOpen) return null;

  return (
    <div
      className="fixed inset-0 bg-gray-500 bg-opacity-75 flex justify-center items-center z-50"
      role="dialog"
      aria-labelledby="delete-modal-title"
      aria-hidden={!isOpen}
    >
      <div className="bg-white p-6 rounded-lg shadow-lg max-w-sm w-full transform transition-all duration-300 ease-in-out scale-100 hover:scale-105">
        <h3
          id="delete-modal-title"
          className="text-xl font-semibold mb-4 text-black"
        >
          Are you sure?
        </h3>
        <p className="mb-4 text-black">
          Are you sure you want to delete this employee? This action cannot be
          undone.
        </p>
        <div className="flex justify-between gap-4">
          <button
            onClick={onClose}
            className="px-4 py-2 bg-gray-200 text-gray-800 rounded-lg hover:bg-gray-300 focus:outline-none focus:ring-2 focus:ring-gray-500"
            aria-label="Cancel deletion"
          >
            Cancel
          </button>
          <button
            onClick={onConfirm}
            className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500"
            aria-label="Confirm deletion"
          >
            Confirm Delete
          </button>
        </div>
      </div>
    </div>
  );
};

export default DeleteModal;
