import { Link } from "react-router-dom";

export default function Unauthorized() {
  return (
    <div className="flex flex-col justify-center items-center h-screen">
      <h1 className="text-4xl font-bold text-red-600 mb-4">403 Unauthorized</h1>
      <p className="text-gray-700 mb-6">
        Sorry, you don’t have permission to view this page.
      </p>
      <Link to="/" className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
        Go Home
      </Link>
    </div>
  );
}
