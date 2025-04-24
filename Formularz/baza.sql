CREATE DATABASE IF NOT EXISTS szkola;
USE szkola;

CREATE TABLE uzytkownicy (
    id INT AUTO_INCREMENT PRIMARY KEY,
    imie VARCHAR(50) NOT NULL,
    nazwisko VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    haslo VARCHAR(255) NOT NULL
);

INSERT INTO uzytkownicy (imie, nazwisko, email, haslo) VALUES
('Anna', 'Kowalska', 'anna@example.com', '123'),
('Jan', 'Nowak', 'jan@example.com', '456'),
('Janina', 'Nowak', 'janina@gmail.com', '456'),
('Janka', 'Abramczyk', 'janka@gmail.com', '456'),
('Jan', 'Kowalski', 'jan.kowalski@gmail.pl', '456'),
('Jan', 'Adamowicz', 'jan.adamowicz@example.com', '456'),
('Ignacy', 'Nowak', 'ignacy@example.com', '456'),
('Antonina', 'Nowak', 'antonina@example.com', '456'),
('Gabriel', 'Nowak', 'gabriel@example.com', '456');