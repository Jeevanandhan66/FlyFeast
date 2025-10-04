export default function Register() {
  return (
    <div className="flex flex-col items-center justify-center w-full h-full">

      <h1 className="text-2xl font-bold mb-4">Register</h1>
      <form className="w-full max-w-sm bg-white shadow-md rounded px-8 pt-6 pb-8">
        <input
          type="text"
          placeholder="Full Name"
          className="w-full mb-4 p-2 border rounded"
        />
        <input
          type="email"
          placeholder="Email"
          className="w-full mb-4 p-2 border rounded"
        />
        <input
          type="password"
          placeholder="Password"
          className="w-full mb-4 p-2 border rounded"
        />
        <button
          type="submit"
          className="w-full bg-green-600 text-white py-2 rounded hover:bg-green-700"
        >
          Register
        </button>
      </form>
    </div>
  );
}
