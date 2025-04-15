import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import DeleteModal from "../components/Modal/DeleteModal"; // Importe a modal aqui
import { employeeService } from "../services/employeeService";
import { formatDateWithDiff } from "../utils/dateFormatter";
import { useNavigate } from "react-router-dom";
import { useState } from "react";

function HomePage() {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const pageSize = 3;
  const [selectedEmployeeId, setSelectedEmployeeId] = useState<string | null>(
    null
  );
  const [isModalOpen, setIsModalOpen] = useState(false);

  const { data, isLoading } = useQuery({
    queryKey: ["employees", page],
    queryFn: () =>
      employeeService.getPaged({
        page: page,
        pageSize: pageSize,
      }),
  });

  const employees = data?.items ?? [];
  const totalPages = data?.totalPages ?? 0;
  const queryClient = useQueryClient();

  const handlePrev = () => {
    setPage((prevPage) => (prevPage > 1 ? prevPage - 1 : prevPage));
  };

  const handleNext = () => {
    setPage((prevPage) => (prevPage < totalPages ? prevPage + 1 : prevPage));
  };

  const mutation = useMutation({
    mutationFn: (id: string) => employeeService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries(["employees", page]);

      if (employees.length === 1 && page > 1) {
        setPage((prevPage) => prevPage - 1);
      }
    },
    onError: (error: any) => {
      alert("Error deleting employee: " + error.message);
    },
  });

  const handleDelete = (id: string) => {
    setSelectedEmployeeId(id);
    setIsModalOpen(true);
  };

  const handleConfirmDelete = () => {
    if (selectedEmployeeId) {
      mutation.mutate(selectedEmployeeId);
    }
    setIsModalOpen(false);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
  };

  return (
    <div className="p-6 w-full h-screen flex flex-col space-y-6">
      <div className="flex justify-between items-center mb-6 w-full">
        <h1 className="text-2xl font-bold">Employee List</h1>
        <button
          onClick={() => navigate("/employees/new")}
          className="bg-green-600 text-white px-6 py-3 rounded-full shadow-lg transform transition-all duration-300 hover:scale-105 hover:shadow-2xl focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-opacity-50"
        >
          <span className="text-lg font-semibold">+ New Employee</span>
        </button>
      </div>

      {isLoading ? (
        <p>Loading...</p>
      ) : (
        <div className="grid gap-4 grid-cols-1 w-full">
          {employees.map((e) => (
            <div
              key={e.id}
              className="bg-white p-4 shadow rounded flex flex-col items-start w-full"
            >
              <div className="flex items-center w-full gap-4">
                <img
                  src={`https://ui-avatars.com/api/?name=${e.firstName}+${e.lastName}`}
                  className="w-12 h-12 md:w-16 md:h-16 rounded-full"
                />
                <div>
                  <p className="font-medium text-black">
                    {e.firstName} {e.lastName}
                  </p>
                  <p className="text-sm text-gray-500">{e.departmentName}</p>
                  <p className="text-sm text-gray-500">
                    {formatDateWithDiff(e.hireDate)}
                  </p>
                </div>
                <div className="ml-auto flex gap-2">
                  <button
                    onClick={() => navigate(`/employees/${e.id}`)}
                    className="px-3 py-1 bg-blue-600 text-white rounded hover:bg-blue-700"
                  >
                    View Details
                  </button>
                  <button
                    onClick={() => handleDelete(e.id)}
                    className="px-3 py-1 bg-red-600 text-white rounded hover:bg-red-700"
                  >
                    ❌
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      <div className="flex justify-center gap-4 pt-4 w-full">
        <button
          disabled={page === 1}
          onClick={handlePrev}
          className="px-3 py-1 bg-blue-600 rounded disabled:opacity-50"
        >
          ⬅ Prev
        </button>
        <span>
          Page {page} of {totalPages}
        </span>
        <button
          disabled={page === totalPages}
          onClick={handleNext}
          className="px-3 py-1 bg-blue-600 rounded disabled:opacity-50"
        >
          Next ➡
        </button>
      </div>

      {/* Modal de confirmação de exclusão */}
      <DeleteModal
        isOpen={isModalOpen}
        onClose={handleCloseModal}
        onConfirm={handleConfirmDelete}
      />
    </div>
  );
}

export default HomePage;
