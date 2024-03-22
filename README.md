# inventory-api

This is an api for my inventory web application.
Built in .NET 8.

## Database Schema

-- This script was generated by the ERD tool in pgAdmin 4.
-- Please log an issue at https://redmine.postgresql.org/projects/pgadmin4/issues/new if you find any bugs, including reproduction steps.
BEGIN;

CREATE TABLE IF NOT EXISTS public."CATEGORIES"
(
"CATEGORY_ID" integer NOT NULL DEFAULT nextval('"CATEGORIES_CATEGORY_ID_seq"'::regclass),
"CATEGORY_NAME" character varying(128) COLLATE pg_catalog."default" NOT NULL,
"CATEGORY_DESCRIPTION" character varying(2048) COLLATE pg_catalog."default" NOT NULL,
CONSTRAINT "CATEGORIES_pkey" PRIMARY KEY ("CATEGORY_ID")
);

COMMENT ON TABLE public."CATEGORIES"
IS 'This table is used to categorize items.';

CREATE TABLE IF NOT EXISTS public."EXPENSES"
(
"EXPENSE_ID" integer NOT NULL DEFAULT nextval('"EXPENSES_EXPENSE_ID_seq"'::regclass),
"EXPENSE_DESCRIPTION" character varying(2048) COLLATE pg_catalog."default" NOT NULL,
"EXPENSE_AMOUNT" numeric(16, 2) NOT NULL,
"EXPENSE_DATE" date NOT NULL,
"CATEGORY_ID" integer,
"INVENTORY_ID" integer,
"MARKETPLACE_ID" integer,
"LAST_UPDATED" time with time zone NOT NULL,
CONSTRAINT "EXPENSES_pkey" PRIMARY KEY ("EXPENSE_ID")
);

CREATE TABLE IF NOT EXISTS public."INVENTORY"
(
"INVENTORY_ID" integer NOT NULL DEFAULT nextval('"INVENTORY_INVENTORY_ID_seq"'::regclass),
"PRODUCT_ID" integer NOT NULL,
"ACTION" character varying(32) COLLATE pg_catalog."default" NOT NULL,
"QUANTITY_CHANGED" integer NOT NULL,
"TIMESTAMP" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
"LAST_UPDATED" time with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
CONSTRAINT "INVENTORY_pkey" PRIMARY KEY ("INVENTORY_ID")
);

CREATE TABLE IF NOT EXISTS public."MARKETPLACES"
(
"MARKETPLACE_ID" integer NOT NULL DEFAULT nextval('"MARKETPLACES_MARKETPLACE_ID_seq"'::regclass),
"MARKETPLACE_NAME" character varying(128) COLLATE pg_catalog."default" NOT NULL,
"MARKETPLACE_DESCRIPTION" character varying(2048) COLLATE pg_catalog."default" NOT NULL,
CONSTRAINT "MARKETPLACES_pkey" PRIMARY KEY ("MARKETPLACE_ID")
);

CREATE TABLE IF NOT EXISTS public."PRODUCTS"
(
"PRODUCT_ID" integer NOT NULL DEFAULT nextval('"PRODUCTS_PRODUCT_ID_seq"'::regclass),
"PRODUCT_NAME" character varying(128) COLLATE pg_catalog."default" NOT NULL,
"PRODUCT_DESCRIPTION" character varying(2048) COLLATE pg_catalog."default" NOT NULL,
"PRODUCT_PRICE" numeric(16, 2) NOT NULL,
"PRODUCT_QUANTITY" integer NOT NULL DEFAULT 0,
"CATEGORY_ID" integer,
CONSTRAINT "PRODUCTS_pkey" PRIMARY KEY ("PRODUCT_ID")
);

COMMENT ON TABLE public."PRODUCTS"
IS 'This table stores the product related information.';

CREATE TABLE IF NOT EXISTS public."SALES"
(
"SALE_ID" integer NOT NULL DEFAULT nextval('"SALES_SALE_ID_seq"'::regclass),
"PRODUCT_ID" integer NOT NULL,
"QUANTITY_SOLD" integer NOT NULL,
"SALE_AMOUNT" numeric(16, 2) NOT NULL,
"SALE_DATE" date NOT NULL,
"INVENTORY_ID" integer NOT NULL,
"MARKETPLACE_ID" integer NOT NULL,
"PROFIT_AMOUNT" numeric(16, 2) NOT NULL,
"LAST_UPDATED" time with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
CONSTRAINT "SALES_pkey" PRIMARY KEY ("SALE_ID")
);

ALTER TABLE IF EXISTS public."EXPENSES"
ADD CONSTRAINT "CATEGORY_ID" FOREIGN KEY ("CATEGORY_ID")
REFERENCES public."CATEGORIES" ("CATEGORY_ID") MATCH SIMPLE
ON UPDATE NO ACTION
ON DELETE NO ACTION
NOT VALID;

ALTER TABLE IF EXISTS public."EXPENSES"
ADD CONSTRAINT "INVENTORY_ID" FOREIGN KEY ("INVENTORY_ID")
REFERENCES public."INVENTORY" ("INVENTORY_ID") MATCH SIMPLE
ON UPDATE NO ACTION
ON DELETE NO ACTION
NOT VALID;

ALTER TABLE IF EXISTS public."EXPENSES"
ADD CONSTRAINT "MARKETPLACE_ID" FOREIGN KEY ("MARKETPLACE_ID")
REFERENCES public."MARKETPLACES" ("MARKETPLACE_ID") MATCH SIMPLE
ON UPDATE NO ACTION
ON DELETE NO ACTION
NOT VALID;

ALTER TABLE IF EXISTS public."INVENTORY"
ADD CONSTRAINT "PRODUCT_ID" FOREIGN KEY ("PRODUCT_ID")
REFERENCES public."PRODUCTS" ("PRODUCT_ID") MATCH SIMPLE
ON UPDATE NO ACTION
ON DELETE NO ACTION
NOT VALID;

ALTER TABLE IF EXISTS public."PRODUCTS"
ADD CONSTRAINT "CATEGORY_ID" FOREIGN KEY ("CATEGORY_ID")
REFERENCES public."CATEGORIES" ("CATEGORY_ID") MATCH SIMPLE
ON UPDATE NO ACTION
ON DELETE NO ACTION
NOT VALID;

ALTER TABLE IF EXISTS public."SALES"
ADD CONSTRAINT "INVENTORY_ID" FOREIGN KEY ("INVENTORY_ID")
REFERENCES public."INVENTORY" ("INVENTORY_ID") MATCH SIMPLE
ON UPDATE NO ACTION
ON DELETE NO ACTION
NOT VALID;

ALTER TABLE IF EXISTS public."SALES"
ADD CONSTRAINT "MARKETPLACE_ID" FOREIGN KEY ("MARKETPLACE_ID")
REFERENCES public."MARKETPLACES" ("MARKETPLACE_ID") MATCH SIMPLE
ON UPDATE NO ACTION
ON DELETE NO ACTION
NOT VALID;

ALTER TABLE IF EXISTS public."SALES"
ADD CONSTRAINT "PRODUCT_ID" FOREIGN KEY ("PRODUCT_ID")
REFERENCES public."PRODUCTS" ("PRODUCT_ID") MATCH SIMPLE
ON UPDATE NO ACTION
ON DELETE NO ACTION
NOT VALID;

END;

## Request Documents

### Products

- Get
  - Get Request will return a list of the products.
  - URL is URL/Products

### Categories

- Get
  - Get Request will return a list of the categories.
  - URL is URL/Categories
