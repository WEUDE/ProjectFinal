﻿using FitnessReservatieBL.DTO;
using FitnessReservatieBL.Interfaces;
using FitnessReservatieDL.Exceptions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            SqlConnection connection = new SqlConnection(_connectiestring);
            return connection;
        }
        public KlantInfo SelecteerKlant(int? klantnummer, string mailadres)
        {
            if ((!klantnummer.HasValue) && (string.IsNullOrEmpty(mailadres)) == true) throw new KlantRepoADOException("KlantRepoADO - SelecteerKlant - 'Ongeldige input'");
            string query = "SELECT * FROM klant";
            if (klantnummer.HasValue) query += "WHERE klantnummer=@klantnummer";
            else query += "WHERE mailadres=@mailadres";
            SqlConnection conn = GetConnection();
            KlantInfo klantinfo = null;
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                conn.Open();
                try
                {
                    if (klantnummer.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@id", klantnummer);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@mailadres", mailadres);
                    }
                    IDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = (int)reader["klantnummer"];
                        string naam = (string)reader["naam"];
                        string voornaam = (string)reader["voornaam"];
                        string mail = (string)reader["mailadres"];
                        string abonnementtype = (string)reader["abonnementtype"];

                        klantinfo = new KlantInfo(id, naam, voornaam, mail, abonnementtype);
                    }
                    return klantinfo;
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
    }
}