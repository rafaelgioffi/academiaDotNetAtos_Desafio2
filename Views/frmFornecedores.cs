﻿using System.Data;

namespace MiniERP
{
    public partial class frmFornecedores : Form
    {
        public frmFornecedores()
        {
            InitializeComponent();
            AtualizaForn();
        }

        private void AtualizaForn()
        {
            try
            {
                Banco bd = new Banco();
                DataTable dt = new DataTable();

                dt = bd.Consulta("SELECT FornId 'ID', FornNome 'Nome', FornCnpj 'CNPJ' FROM Fornecedores");

                dgvForn.DataSource = dt;

                lblStatus.Text = $"{dgvForn.RowCount} fornecedores cadastrados";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao recuperar os dados do banco de dados...");
            }
        }

        private void LimparCampos()
        {
            txtNomeForn.Clear();
            txtCnpjForn.Clear();
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            AtualizaForn();
            txtPesquisa.Clear();
        }

        private void frmFornecedores_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmMiniErp frmPrincipal = new frmMiniErp();
            frmPrincipal.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frmMiniErp frmPrincipal = new frmMiniErp();
            frmPrincipal.Show();
        }

        private void txtPesquisa_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtPesquisa.Text.Length > 3)
            {
                Banco bd = new Banco();
                DataTable dt = new DataTable();
                dt = bd.Consulta($"SELECT FornId 'ID', FornNome 'Nome', FornCnpj 'CNPJ' FROM Fornecedores WHERE FornNome LIKE '%{txtPesquisa.Text}%'");

                dgvForn.DataSource = dt;
            }
            else
            {
                AtualizaForn();
            }
        }

        private void dgvForn_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            btnExcluiForn.Enabled = true;
            btnEditarForn.Enabled = true;
        }

        private void btnCadastrarForn_Click(object sender, EventArgs e)
        {
            grpCadForn.Visible = true;
        }

        private void btnEditarForn_Click(object sender, EventArgs e)
        {
            frmEditForn fornecedor = new frmEditForn();
            fornecedor.txtFornId.Text = dgvForn.SelectedRows[0].Cells[0].Value.ToString();
            fornecedor.txtNomeForn.Text = dgvForn.SelectedRows[0].Cells[1].Value.ToString();
            fornecedor.txtCnpjForn.Text = dgvForn.SelectedRows[0].Cells[2].Value.ToString();

            fornecedor.Show();
            this.Hide();
        }

        private void btnCadForn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNomeForn.Text))
            {
                MessageBox.Show("O campo Nome é de preenchimento obrigatório");
                txtNomeForn.Focus();
            }
            else if (string.IsNullOrWhiteSpace(txtCnpjForn.Text))
            {
                MessageBox.Show("O campo CNPJ é de preenchimento obrigatório");
                txtCnpjForn.Focus();
            }
            else
            {
                Fornecedores fornecedor = new Fornecedores();
                fornecedor.FornNome = txtNomeForn.Text;
                fornecedor.FornCnpj = txtCnpjForn.Text;

                if (fornecedor.CadastraForn())
                {
                    MessageBox.Show("Fornecedor cadastrado com sucesso!");
                    AtualizaForn();
                    LimparCampos();
                    grpCadForn.Visible = false;
                }
                else
                {
                    MessageBox.Show("Falha ao cadastrar o fornecedor. Tente novamente...");
                    AtualizaForn();
                }
            }
        }

        private void btnExcluiForn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Tem certeza que deseja excluir o fornecedor '{dgvForn.SelectedRows[0].Cells[1].Value.ToString()}' do ID {dgvForn.SelectedRows[0].Cells[0].Value.ToString()}?", "Exclusão de fornecedor", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Fornecedores fornecedor = new Fornecedores();
                fornecedor.FornId = int.Parse(dgvForn.SelectedRows[0].Cells[0].Value.ToString());
                fornecedor.FornNome = dgvForn.SelectedRows[0].Cells[2].Value.ToString();

                fornecedor.ConsultaForn(fornecedor.FornId);

                if (fornecedor == null)
                {
                    MessageBox.Show($"Erro ao excluir o fornecedor '{fornecedor.FornNome}'.\nTente novamente");
                    return;
                }
                bool retorno = fornecedor.ExcluiForn();

                if (retorno)
                {
                    AtualizaForn();
                    MessageBox.Show($"'{fornecedor.FornNome}' excluído com sucesso!", "fornecedor excluído");
                }
                else
                {
                    MessageBox.Show($"Erro ao excluir o fornecedor '{fornecedor.FornNome}'.\nTente novamente");
                    AtualizaForn();
                }
            }
        }
    }
}
