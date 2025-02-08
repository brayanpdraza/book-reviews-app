import React from "react";

const Footer = () => {
  return (
    <footer className="bg-gray-800 text-white text-center py-4 mt-10">
      <p>&copy; {new Date().getFullYear()} Book Review App Brayan Pedraza - Todos los derechos reservados</p>
    </footer>
  );
};

export default Footer;
