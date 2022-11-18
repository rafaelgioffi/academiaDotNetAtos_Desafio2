﻿using MiniERP.Classes;
using System.Data;

namespace MiniERP.Views
{
    public partial class frmNotas : Form
    {
        public frmNotas()
        {
            InitializeComponent();
            AtualizaNotas();
        }
        private void AtualizaNotas()
        {
            try
            {
                Banco bd = new Banco();
                DataTable dt = new DataTable();

                dt = bd.Consulta("SELECT NotId 'ID', NotInfo 'Observações', NotDataHora 'Data e Hora', CliId 'ID do Cliente', ProdId 'ID do Produto' FROM Notas");

                dgvNotas.DataSource = dt;

                lblStatus.Text = $"{dgvNotas.RowCount} notas cadastradas";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao recuperar os dados do banco de dados...");
            }
        }

        private void LimparCampos()
        {
            txtInfoNota.Clear();
            txtIdCli.Value = 1;
            txtIdProd.Value = 1;
        }

        private void txtPesquisa_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtPesquisa.Text.Length > 3)
            {
                Banco bd = new Banco();
                DataTable dt = new DataTable();
                dt = bd.Consulta($"SELECT NotId 'ID', NotInfo 'Observações', NotDataHora 'Data e Hora', CliId 'ID do Cliente', ProdId 'ID do Produto' FROM Notas WHERE NotInfo LIKE '%{txtPesquisa.Text}%'");

                dgvNotas.DataSource = dt;
            }
            else
            {
                AtualizaNotas();
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            AtualizaNotas();
            txtPesquisa.Clear();
        }

        private void dgvNotas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnEditarNota.Enabled = true;
            btnExcluiNota.Enabled = true;
        }

        private void btnCadastrarNota_Click(object sender, EventArgs e)
        {
            grpCadNota.Visible = true;
            btnCadastrarNota.Enabled = false;
        }

        private void btnEditarNota_Click(object sender, EventArgs e)
        {
            frmEditNot nota = new frmEditNot();
            nota.txtIdNot.Text = dgvNotas.SelectedRows[0].Cells[0].Value.ToString();
            nota.txtInfoNota.Text = dgvNotas.SelectedRows[0].Cells[1].Value.ToString();
            nota.txtIdCli.Text = dgvNotas.SelectedRows[0].Cells[3].Value.ToString();
            nota.txtIdProd.Text = dgvNotas.SelectedRows[0].Cells[4].Value.ToString();

            nota.Show();
            this.Hide();
        }

        private void btnExcluiNota_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Tem certeza que deseja excluir a nota ID '{dgvNotas.SelectedRows[0].Cells[0].Value.ToString()}'?", "Exclusão de nota", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Notas nota = new Notas();
                nota.NotId = dgvNotas.SelectedRows[0].Cells[0].Value.ToString();

                nota.ConsultaNot(nota.NotId);

                if (nota == null)
                {
                    MessageBox.Show($"Erro ao excluir a nota '{nota.NotId}'.\nTente novamente");
                    return;
                }
                bool retorno = nota.ExcluiNot();

                if (retorno)
                {
                    AtualizaNotas();
                    MessageBox.Show($"'{nota.NotId}' excluída com sucesso!", "Nota excluída");
                }
                else
                {
                    MessageBox.Show($"Erro ao excluir a nota '{nota.NotId}'.\nTente novamente");
                    AtualizaNotas();
                }
            }
        }

        private void btnCadCli_Click(object sender, EventArgs e)
        {
            //tentar converter os id's do cliente e prooduto
            int cliId, prodId;
            int.TryParse(txtIdCli.Text, out cliId);
            int.TryParse(txtIdProd.Text, out prodId);

            //tratar a data e hora
            DateTime datahora = DateTime.Now;
            string datahoraFormat = $"{datahora.Day}-{datahora.Month}-{datahora.Year} {datahora.Hour}:{datahora.Minute}:{datahora.Second}";            

            //criar a primary key...
            string NotId = datahora.ToString();
            NotId = NotId.Replace("/","");
            NotId = NotId.Replace(":","");
            NotId = NotId.Replace(" ","");
            NotId += cliId.ToString() + prodId.ToString();                     

            if(cliId <= 0)
            {
                MessageBox.Show("O campo 'ID do Cliente' é de preenchimento obrigatório");
                txtIdCli.Focus();
            }
            else if (prodId <= 0)
            {
                MessageBox.Show("O campo 'ID do Produto' é de preenchimento obrigatório");
                txtIdProd.Focus();
            }
            else
            {
                Notas nota = new Notas();
                nota.NotId = NotId;
                nota.NotInfo = txtInfoNota.Text;
                nota.NotDataHora = datahoraFormat;
                nota.CliId = int.Parse(txtIdCli.Value.ToString());
                nota.ProdId = int.Parse(txtIdProd.Value.ToString());

                if (nota.CadastraNota())
                {
                    MessageBox.Show("Nota cadastrada com sucesso!");
                    AtualizaNotas();
                    LimparCampos();
                    grpCadNota.Visible = false;
                    btnCadastrarNota.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Falha ao cadastrar a nota. Tente novamente...");
                    AtualizaNotas();
                }
            }
        }

        private void frmNotas_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmMiniErp frmMini = new frmMiniErp();
            frmMini.Show();
        }
    }
}
