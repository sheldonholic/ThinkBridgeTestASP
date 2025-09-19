-- Create tables
CREATE TABLE IF NOT EXISTS Invoices (
                                        InvoiceID INTEGER PRIMARY KEY,
                                        CustomerName TEXT
);

CREATE TABLE IF NOT EXISTS InvoiceItems (
                                            ItemID INTEGER PRIMARY KEY,
                                            InvoiceID INTEGER,
                                            Name TEXT,
                                            Price REAL,
                                            FOREIGN KEY (InvoiceID) REFERENCES Invoices(InvoiceID)
    );

-- Insert sample invoices
INSERT INTO Invoices (InvoiceID, CustomerName) VALUES (1, 'John Doe');
INSERT INTO Invoices (InvoiceID, CustomerName) VALUES (2, 'Alice Smith');

-- Insert items for Invoice 1
INSERT INTO InvoiceItems (ItemID, InvoiceID, Name, Price) VALUES (1, 1, 'Widget A', 19.99);
INSERT INTO InvoiceItems (ItemID, InvoiceID, Name, Price) VALUES (2, 1, 'Widget B', 29.99);

-- Insert items for Invoice 2
INSERT INTO InvoiceItems (ItemID, InvoiceID, Name, Price) VALUES (3, 2, 'Service X', 99.50);
INSERT INTO InvoiceItems (ItemID, InvoiceID, Name, Price) VALUES (4, 2, 'Support Y', 49.00);
