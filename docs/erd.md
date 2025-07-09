erDiagram
    %% === CORE USER MANAGEMENT ===
    USER {
        UUID id PK
        string full_name
        string email
        string phone
        string password_hash
        string role
        boolean is_active
        datetime created_at
    }

    %% === PET MANAGEMENT ===
    PET {
        UUID id PK
        UUID owner_id FK
        string name
        string species
        string breed
        date birth_date
        decimal weight
        string photo_url
        datetime created_at
    }
    USER ||--o{ PET : owns

    %% === SERVICES ===
    SERVICE {
        UUID id PK
        string name
        text description
        int duration_minutes
        decimal price
        boolean is_active
    }

    %% === APPOINTMENTS ===
    APPOINTMENT {
        UUID id PK
        UUID pet_id FK
        UUID doctor_id FK
        UUID service_id FK
        datetime appointment_time
        string status
        text notes
        datetime created_at
    }
    PET ||--o{ APPOINTMENT : "booked for"
    USER ||--o{ APPOINTMENT : "doctor handles"
    SERVICE ||--o{ APPOINTMENT : "uses"

    %% === MEDICAL RECORDS ===
    MEDICAL_RECORD {
        UUID id PK
        UUID appointment_id FK
        UUID pet_id FK
        UUID doctor_id FK
        date visit_date
        text diagnosis
        text treatment_notes
        text prescription
        datetime created_at
    }
    APPOINTMENT ||--|| MEDICAL_RECORD : "results in"
    PET ||--o{ MEDICAL_RECORD : has
    USER ||--o{ MEDICAL_RECORD : "doctor writes"

    %% === VACCINATION TRACKING ===
    VACCINATION {
        UUID id PK
        UUID pet_id FK
        string vaccine_name
        date due_date
        date completed_date
        string status
        text notes
    }
    PET ||--o{ VACCINATION : needs

    %% === REMINDERS ===
    REMINDER {
        UUID id PK
        UUID user_id FK
        UUID pet_id FK
        string type
        string title
        text message
        datetime send_at
        string status
    }
    USER ||--o{ REMINDER : receives
    PET ||--o{ REMINDER : about

    %% === FEEDBACK ===
    FEEDBACK {
        UUID id PK
        UUID appointment_id FK
        UUID customer_id FK
        int rating
        text comment
        boolean approved
        datetime created_at
    }
    APPOINTMENT ||--o{ FEEDBACK : creates
    USER ||--o{ FEEDBACK : writes

    %% === INVOICES ===
    INVOICE {
        UUID id PK
        UUID appointment_id FK
        decimal total_amount
        string status
        datetime created_at
        datetime paid_at
    }
    APPOINTMENT ||--|| INVOICE : generates
