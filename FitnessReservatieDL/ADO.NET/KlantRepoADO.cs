﻿using FitnessReservatieBL.Domeinen;
using FitnessReservatieBL.DTO;
using FitnessReservatieBL.Interfaces;
using FitnessReservatieDL.Exceptions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace FitnessReservatieDL.ADO.NET
{
    public class KlantRepoADO : IKlantRepository
    {
        private string _connectiestring;

        public KlantRepoADO(string connectiestring)
        {
            this._connectiestring = connectiestring;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectiestring);
        }

        public Klant SelecteerKlant(int? klantnummer, string mailadres)
        {
            if ((!klantnummer.HasValue) && (string.IsNullOrEmpty(mailadres)) == true) throw new KlantRepoADOException("KlantRepoADO - SelecteerKlant - 'Ongeldige input'");
            string query = "SELECT klantnummer,naam,voornaam,mailadres FROM Klant ";
            if (klantnummer.HasValue) query += "WHERE klantnummer=@klantnummer";
            else query += "WHERE mailadres=@mailadres";
            Klant klant = null;
            SqlConnection conn = GetConnection();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                conn.Open();
                try
                {
                    if (klantnummer.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@klantnummer", klantnummer);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@mailadres", mailadres);
                    }
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        klant = new Klant((int)reader["klantnummer"], (string)reader["naam"], (string)reader["voornaam"], (string)reader["mailadres"]);
                    }
                    reader.Close();
                    return klant;
                }
                catch (Exception ex)
                {
                    throw new KlantRepoADOException("KlantRepoADO - SelecteerKlant", ex);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public IReadOnlyList<DTOKlantReservatieInfo> GeefKlantReservaties(int klantnummer)
        {
            if (klantnummer <= 0) throw new KlantRepoADOException("KlantRepoADO - GeefKlantReservaties - 'Ongeldige input'");

            string query1 = "SELECT Count(*) FROM Reservatie WHERE klantnummer=@klantnummer";

            string query2 = "SELECT r.reservatienummer, r.datum, i.beginuur, i.einduur, t.toestelnaam FROM Reservatie r " +
            "LEFT JOIN ReservatieInfo i ON r.reservatienummer = i.reservatienummer " +
            "LEFT JOIN toestel t ON i.toestelnummer = t.toestelnummer " +
            "LEFT JOIN Klant k ON r.klantnummer = k.klantnummer " +
            "WHERE r.klantnummer = @klantnummer ORDER BY r.datum, i.beginuur";
            SqlConnection conn = GetConnection();
            List<DTOKlantReservatieInfo> klantenreservaties = new List<DTOKlantReservatieInfo>();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = query1;
                cmd.Parameters.AddWithValue("@klantnummer", klantnummer);
                conn.Open();

                try
                {
                    int entries = (int)cmd.ExecuteScalar();
                    if (entries == 0)
                    {
                        return new List<DTOKlantReservatieInfo>();
                    }
                    else
                    {
                        cmd.CommandText = query2;
                        IDataReader reader = cmd.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                DTOKlantReservatieInfo klantenreservatie = new DTOKlantReservatieInfo((int)reader["reservatienummer"], (DateTime)reader["datum"], (int)reader["beginuur"], (int)reader["einduur"], (string)reader["toestelNaam"]);
                                klantenreservaties.Add(klantenreservatie);
                            }
                            return klantenreservaties.AsReadOnly();
                        }
                        catch (Exception ex)
                        {
                            throw new KlantRepoADOException("KlantRepoADO - GeefKlantReservaties - 'query2'", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new KlantRepoADOException("KlantRepoADO - GeefKlantReservaties - 'query1'", ex);
                }
                finally
                {
                    conn.Close();
                }

            }
        }

        public IReadOnlyList<Klant> ZoekKlanten(int klantnummer, string zoekterm)
        {
            string query = "SELECT klantnummer,naam,voornaam,mailadres FROM Klant ";
            if (klantnummer > 0) query += "WHERE klantnummer=@klantnummer";
            else if (!string.IsNullOrWhiteSpace(zoekterm)) query += "WHERE naam LIKE '%' + @zoekterm + '%' OR voornaam LIKE '%' + @zoekterm + '%'";
            Klant klant = null;
            List<Klant> klanten = new List<Klant>();
            SqlConnection conn = GetConnection();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                conn.Open();
                try
                {
                    if (klantnummer > 0)
                    {
                        cmd.Parameters.AddWithValue("@klantnummer", klantnummer);
                    }
                    else if (zoekterm != null)
                    {
                        cmd.Parameters.AddWithValue("@zoekterm", zoekterm);
                    }
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        klant = new Klant((int)reader["klantnummer"], (string)reader["naam"], (string)reader["voornaam"], (string)reader["mailadres"]);
                        klanten.Add(klant);
                    }
                    reader.Close();
                    return klanten.AsReadOnly();
                }
                catch (Exception ex)
                {
                    throw new KlantRepoADOException("KlantRepoADO - ZoekKlant", ex);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}
