DROP TABLE IF EXISTS tickets CASCADE; 
DROP TABLE IF EXISTS bookings CASCADE; 
DROP TABLE IF EXISTS schedule_seats CASCADE; 
DROP TABLE IF EXISTS schedules CASCADE; 
DROP TABLE IF EXISTS trains CASCADE; 
DROP TABLE IF EXISTS stations CASCADE; 
DROP TABLE IF EXISTS users CASCADE; 
DROP TYPE IF EXISTS ticket_status CASCADE; 
DROP TYPE IF EXISTS payment_status CASCADE; 
DROP TYPE IF EXISTS train_class CASCADE; 
DROP TYPE IF EXISTS user_role CASCADE;

CREATE TYPE user_role AS ENUM ('Admin', 'Customer', 'GateOfficer');
CREATE TYPE train_class AS ENUM ('Ekonomi', 'Bisnis', 'Eksekutif');
CREATE TYPE payment_status AS ENUM ('UNPAID', 'PAID', 'EXPIRED', 'CANCELLED');
CREATE TYPE ticket_status AS ENUM ('UNPAID', 'PAID', 'REDEEMED');

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    role user_role NOT NULL DEFAULT 'Customer',
    nik VARCHAR(20),
    station_code VARCHAR(10),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE stations (
    id SERIAL PRIMARY KEY,
    code VARCHAR(10) UNIQUE NOT NULL,
    name VARCHAR(100) NOT NULL,
    city VARCHAR(50) NOT NULL
);

CREATE TABLE trains (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    class train_class NOT NULL,
    total_seats INT NOT NULL DEFAULT 50
);

CREATE TABLE schedules (
    id SERIAL PRIMARY KEY,
    train_id INT NOT NULL,
    origin_station_id INT NOT NULL,
    destination_station_id INT NOT NULL,
    departure_time TIMESTAMP NOT NULL,
    arrival_time TIMESTAMP NOT NULL,
    price DECIMAL(12, 2) NOT NULL,
    CONSTRAINT fk_schedules_train 
        FOREIGN KEY (train_id) REFERENCES trains(id) ON DELETE CASCADE,
    CONSTRAINT fk_schedules_origin_station 
        FOREIGN KEY (origin_station_id) REFERENCES stations(id) ON DELETE RESTRICT,
    CONSTRAINT fk_schedules_destination_station 
        FOREIGN KEY (destination_station_id) REFERENCES stations(id) ON DELETE RESTRICT
);

CREATE TABLE schedule_seats (
    id SERIAL PRIMARY KEY,
    schedule_id INT NOT NULL,
    seat_number VARCHAR(10) NOT NULL,
    is_available BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT fk_seats_schedule 
        FOREIGN KEY (schedule_id) REFERENCES schedules(id) ON DELETE CASCADE,
    CONSTRAINT uq_schedule_seat 
        UNIQUE (schedule_id, seat_number)
);

CREATE TABLE bookings (
    id SERIAL PRIMARY KEY,
    booking_code VARCHAR(20) UNIQUE NOT NULL,
    user_id INT NOT NULL,
    schedule_id INT NOT NULL,
    total_amount DECIMAL(12, 2) NOT NULL,
    payment_method VARCHAR(50),
    status payment_status NOT NULL DEFAULT 'UNPAID',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_bookings_user 
        FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_bookings_schedule 
        FOREIGN KEY (schedule_id) REFERENCES schedules(id) ON DELETE RESTRICT
);

CREATE TABLE tickets (
    id SERIAL PRIMARY KEY,
    booking_id INT NOT NULL,
    seat_id INT NOT NULL,
    passenger_name VARCHAR(100) NOT NULL,
    passenger_nik VARCHAR(20) NOT NULL,
    redeem_code VARCHAR(20) UNIQUE,
    status ticket_status NOT NULL DEFAULT 'UNPAID',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_tickets_booking 
        FOREIGN KEY (booking_id) REFERENCES bookings(id) ON DELETE CASCADE,
    CONSTRAINT fk_tickets_seat 
        FOREIGN KEY (seat_id) REFERENCES schedule_seats(id) ON DELETE RESTRICT
);

-- Data Dummy
-- Select * from users
INSERT INTO users (username, password, full_name, role, nik, station_code) VALUES
('admin', 'password', 'Administrator Utama', 'Admin', NULL, 'GMR'),
('karyawan', 'password', 'Budi Officer', 'GateOfficer', NULL, 'GMR'),
('customer', 'password', 'Rina', 'Customer', '3171012345670001', NULL);

-- Select * from stations
INSERT INTO stations (code, name, city) VALUES
('GMR', 'Stasiun Gambir', 'Jakarta'),
('PSE', 'Stasiun Pasar Senen', 'Jakarta'),
('BD', 'Stasiun Bandung', 'Bandung'),
('CN', 'Stasiun Cirebon', 'Cirebon'),
('SMT', 'Stasiun Semarang Tawang', 'Semarang'),
('SLO', 'Stasiun Solo Balapan', 'Surakarta'),
('SBY', 'Stasiun Surabaya Pasarturi', 'Surabaya');

-- select * from trains
INSERT INTO trains (name, class, total_seats) VALUES
('Argo Bromo Anggrek', 'Eksekutif', 50),
('Argo Parahyangan', 'Eksekutif', 50),
('Matarmaja', 'Ekonomi', 50),
('Turangga', 'Bisnis', 50);

-- select * from schedules
INSERT INTO schedules (train_id, origin_station_id, destination_station_id, departure_time, arrival_time, price) VALUES
(1, 1, 7, '2026-12-01 08:00:00', '2026-12-01 16:00:00', 550000.00),
(2, 1, 3, '2026-12-01 09:30:00', '2026-12-01 12:30:00', 150000.00),
(3, 2, 7, '2026-12-01 10:00:00', '2026-12-01 22:00:00', 180000.00),
(4, 3, 7, '2026-12-01 18:00:00', '2026-12-02 04:00:00', 320000.00);

-- select * from schedule_seats
INSERT INTO schedule_seats (schedule_id, seat_number, is_available) VALUES
(1, 'EKS-1A', false),
(1, 'EKS-1B', true),
(1, 'EKS-2A', true),
(1, 'EKS-2B', true),
(2, 'EKS-1A', false),
(2, 'EKS-1B', false),
(2, 'EKS-2A', true),
(2, 'EKS-2B', true),
(3, 'EKO-1A', true),
(3, 'EKO-1B', true),
(3, 'EKO-2A', true),
(4, 'BIS-1A', true),
(4, 'BIS-1B', true);

-- select * from bookings
INSERT INTO bookings (booking_code, user_id, schedule_id, total_amount, payment_method, status) VALUES
('TKT-20261201-001', 3, 1, 550000.00, 'Virtual Account BCA', 'PAID'),
('TKT-20261201-002', 3, 2, 300000.00, 'E-Wallet GoPay', 'UNPAID');

-- select * from tickets
INSERT INTO tickets (booking_id, seat_id, passenger_name, passenger_nik, redeem_code, status) VALUES
(1, 1, 'Budi Santoso', '3171012345670001', 'RDM-8X92K1', 'PAID'),
(2, 5, 'Budi Santoso', '3171012345670001', NULL, 'UNPAID'),
(2, 6, 'Eko Santoso', '3171012345670002', NULL, 'UNPAID');
