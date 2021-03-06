﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EMEWE.CarManagement.Commons.CommonClass;
using EMEWE.CarManagement.Entity;
using System.Linq.Expressions;
using EMEWE.CarManagement.DAL;

namespace EMEWE.CarManagement.SystemAdmin
{
    public partial class UnLoadCarManage : Form
    {
        public UnLoadCarManage()
        {
            InitializeComponent();
        }

        public MainForm mf;//主窗体
        /// <summary>
        /// 返回一个空的预置皮重
        /// </summary>
        Expression<Func<View_ICCard_ICCardType, bool>> expr = null;
        /// <summary>
        /// 实例化分页控件
        /// </summary>
        PageControl page = new PageControl();

        List<MenuInfo> list = new List<MenuInfo>();
        /// <summary>
        /// 存放查询条件
        /// </summary>
        private string sqlwhere = " ( BusinessRecord_Type = '" + CommonalityEntity.unloadFirstWeight + "' or BusinessRecord_Type = '" + CommonalityEntity.unloadSecondWeight + "' ) and CarInOutRecord_ID=BusinessRecord_CarInOutRecord_ID ";

        /// <summary>
        /// 用户权限，添加删除，修改
        /// </summary>
        private void userContext()
        {
            if (CommonalityEntity.USERNAME.ToLower() == "emewe")
            {

                tsbtnDel.Enabled = true;
                tsbtnDel.Visible = true;
                btnUpdate.Enabled = true;
                btnUpdate.Visible = true;
                tslbExecl.Enabled = true;
                tslbExecl.Visible = true;
            }
            else
            {

                tsbtnDel.Visible = ControlAttributes.BoolControl("tsbtnDel", "UnLoadCarManage", "Visible");
                tsbtnDel.Enabled = ControlAttributes.BoolControl("tsbtnDel", "UnLoadCarManage", "Enabled");

                btnUpdate.Visible = ControlAttributes.BoolControl("btnUpdate", "UnLoadCarManage", "Visible");
                btnUpdate.Enabled = ControlAttributes.BoolControl("btnUpdate", "UnLoadCarManage", "Enabled");

                tslbExecl.Visible = ControlAttributes.BoolControl("tslbExecl", "UnLoadCarManage", "Visible");
                tslbExecl.Enabled = ControlAttributes.BoolControl("tslbExecl", "UnLoadCarManage", "Enabled");
            }
        }

        ///搜索按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSerch_Click(object sender, EventArgs e)
        {
            try
            {
                sqlwhere = "  ( BusinessRecord_Type = '" + CommonalityEntity.unloadFirstWeight + "' or BusinessRecord_Type = '" + CommonalityEntity.unloadSecondWeight + "' ) and CarInOutRecord_ID=BusinessRecord_CarInOutRecord_ID ";
                string type = this.cobCarType.Text.ToString();
                string carNum = this.txtCarNum.Text.Trim();
                string customerName = this.txtCustomerName.Text.Trim();
                string businessRecord_Type = this.txtBusinessRecordType.Text.Trim();
                string serialnumber = this.txtSerialnumber.Text.Trim();
                string sort = this.txtSort.Text.Trim();

                if (!string.IsNullOrEmpty(type))
                {
                    sqlwhere += String.Format(" and CarType_Name = '{0}' ", type);
                }
                if (!string.IsNullOrEmpty(carNum))
                {
                    sqlwhere += String.Format(" and CarInfo_Name like  '%{0}%'", carNum);
                }
                if (!string.IsNullOrEmpty(customerName))
                {
                    sqlwhere += String.Format(" and CustomerInfo_Name like '%{0}%'", customerName);
                }
                if (!string.IsNullOrEmpty(businessRecord_Type))
                {
                    sqlwhere += String.Format(" and BusinessRecord_Type like  '%{0}%'", businessRecord_Type);
                }
                if (!string.IsNullOrEmpty(serialnumber))
                {
                    sqlwhere += String.Format(" and SmallTicket_Serialnumber like  '%{0}%'", serialnumber);
                }
                if (!string.IsNullOrEmpty(sort))
                {
                    sqlwhere += String.Format(" and SmallTicket_SortNumber like  '%{0}%'", sort);
                }
                if (!string.IsNullOrEmpty(this.txtName.Text.Trim()))
                {
                    sqlwhere += String.Format(" and StaffInfo_Name like  '%{0}%'", this.txtName.Text.Trim());
                }
                if (!string.IsNullOrEmpty(this.txtIndes.Text.Trim()))
                {
                    sqlwhere += String.Format(" and StaffInfo_Identity like  '%{0}%'", this.txtIndes.Text.Trim());
                }
            }
            catch 
            {
                CommonalityEntity.WriteTextLog("ICCardForm.btnSearch_Click异常:" + "".ToString());
            }
            finally
            {
                LogInfoLoad("");
            }
        }

        /// <summary>
        /// 初始加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnLoadCarManage_Load(object sender, EventArgs e)
        {
            userContext();
            BindCarType();
            tscbxPageSize.SelectedIndex = 1;
            //  LogInfoLoad("");
        }

        /// <summary>
        /// 绑定车辆类型
        /// </summary>
        private void BindCarType()
        {
            string sql = "select * from CarType where CarType_State='启动'";
            this.cobCarType.DataSource = CarTypeDAL.GetViewCarTypeName(sql);
            if (this.cobCarType.DataSource != null)
            {
                this.cobCarType.DisplayMember = "CarType_Name";
                this.cobCarType.ValueMember = "CarType_ID";
                cobCarType.SelectedIndex = -1;
            }

        }
        /// <summary>
        /// 清空文本框事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            Empty();
        }
        /// <summary>
        /// 清空文本框信息
        /// </summary>
        private void Empty()
        {
            cobCarType.SelectedIndex = -1;
            txtBusinessRecordType.Text = "";
            txtCarNum.Text = "";
            txtCustomerName.Text = "";
            txtName.Text = "";
            txtIndes.Text = "";
            this.txtCar_Num.Text = "";
            this.txtperson.Text = "";
            this.txtRemark.Text = "";
            this.txtWeight.Text = "";
            this.cmbCar_Type.SelectedIndex = -1;
            list.Clear();
            btnUpdate.Enabled = false;
        }
        #region 分页和加载DataGridView
        /// <summary>
        /// 取消全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tslNotChecked()
        {
            for (int i = 0; i < this.gdvUnLoadCarMange.Rows.Count; i++)
            {
                gdvUnLoadCarMange.Rows[i].Selected = false;
            }
        }
        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tslAllChecked()
        {
            for (int i = 0; i < gdvUnLoadCarMange.Rows.Count; i++)
            {
                this.gdvUnLoadCarMange.Rows[i].Selected = true;
            }
        }
        /// <summary>
        /// 分页控件事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bdnInfo_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "tslSelectAll")//全选
            {
                tslAllChecked();
                return;
            }
            if (e.ClickedItem.Name == "tslNotSelect")//取消全选
            {
                tslNotChecked();
                return;
            }
            if (e.ClickedItem.Name == "tsbtnDel")//删除
            {
                tsbtnDel_Click();
            }
            if (e.ClickedItem.Name == "tslbExecl")//导出Execl
            {
                tsbExecl_Click();
                return;
            }
            LogInfoLoad(e.ClickedItem.Name);
        }
        /// <summary>
        /// 选择每页条数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tscbxPageSize2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LogInfoLoad("");
        }

        /// <summary>
        /// 加载信息
        /// </summary>
        public void LogInfoLoad(string strClickedItemName)
        {
            page.BindBoundControl(gdvUnLoadCarMange, strClickedItemName, tstbPageCurrent, tslPageCount, tslNMax, tscbxPageSize, "View_CarState,BusinessRecord ", "case BusinessRecord_UnloadEmpower when '1' then '是' else '否' end as UnloadEmpower,*", "BusinessRecord_ID", "BusinessRecord_ID", 1, sqlwhere, true);
        }
        #endregion

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.txtperson.Text.Trim() == "")
                {
                    MessageBox.Show("过磅操作人不能为空！");
                    return;
                }
                if (this.txtWeight.Text.Trim() == "")
                {
                    MessageBox.Show("过磅重量不能为空！");
                    return;
                }
                if (this.txtRemark.Text.Trim() == "")
                {
                    MessageBox.Show("修改原因不能为空！");
                    return;
                }
                if (this.gdvUnLoadCarMange.SelectedRows.Count > 0)//选中行
                {
                    if (gdvUnLoadCarMange.SelectedRows.Count > 1)
                    {
                        MessageBox.Show("修改只能选中一行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        Expression<Func<BusinessRecord, bool>> pc = n => n.BusinessRecord_ID == int.Parse(this.gdvUnLoadCarMange.SelectedRows[0].Cells["BusinessRecord_ID"].Value.ToString());
                        string id = "";
                        string strfront = "";
                        string strcontent = "";
                        Action<BusinessRecord> ap = s =>
                        {
                            strfront = s.BusinessRecord_WeightPerson + "," + s.BusinessRecord_Weight + "," + s.BusinessRecord_Remark;
                            s.BusinessRecord_WeightPerson = this.txtperson.Text;
                            s.BusinessRecord_Weight = Convert.ToDouble(this.txtWeight.Text.Trim());
                            s.BusinessRecord_Remark = this.txtRemark.Text.Trim();
                            strcontent = s.BusinessRecord_WeightPerson + "," + s.BusinessRecord_Weight + "," + s.BusinessRecord_Remark;
                            id = s.BusinessRecord_ID.ToString();
                        };

                        if (BusinessRecordDAL.Update(pc, ap))
                        {
                            MessageBox.Show("修改成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CommonalityEntity.WriteLogData("修改", "更新车牌号为：" + txtCar_Num.Text.Trim() + "的过磅信息；修改前：" + strfront + "；修改后：" + strcontent, CommonalityEntity.USERNAME);//添加操作日志
                        }
                        else
                        {
                            MessageBox.Show("修改失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch 
            {
                CommonalityEntity.WriteTextLog("地磅房管理 btnUpdate_Click()" + "".ToString());
            }
            finally
            {
                LogInfoLoad("");
                Empty();
            }

        }
        /// <summary>
        /// 双击组件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gdvUnLoadCarMange_DoubleClick(object sender, EventArgs e)
        {
            this.btnUpdate.Enabled = true;
            if (this.gdvUnLoadCarMange.SelectedRows.Count > 0)//选中行
            {
                if (gdvUnLoadCarMange.SelectedRows.Count > 1)
                {
                    MessageBox.Show("修改只能选中一行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    //修改的值
                    int ID = int.Parse(this.gdvUnLoadCarMange.SelectedRows[0].Cells["BusinessRecord_ID"].Value.ToString());
                    this.cmbCar_Type.Text = this.gdvUnLoadCarMange.SelectedRows[0].Cells["CarType_Name"].Value.ToString();
                    this.txtCar_Num.Text = this.gdvUnLoadCarMange.SelectedRows[0].Cells["CarInfo_Name"].Value.ToString();
                    this.txtperson.Text = this.gdvUnLoadCarMange.SelectedRows[0].Cells["BusinessRecord_WeightPerson"].Value.ToString();
                    this.txtWeight.Text = this.gdvUnLoadCarMange.SelectedRows[0].Cells["BusinessRecord_Weight"].Value.ToString(); 
                }
            }
            else
            {
                MessageBox.Show("请选择要修改的行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        /// <summary>
        /// 删除事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbtnDel_Click()
        {
            try
            {
                int j = 0;
                if (gdvUnLoadCarMange.SelectedRows.Count > 0)//选中删除
                {
                    if (MessageBox.Show("确定要删除吗？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        //选中数量
                        int count = gdvUnLoadCarMange.SelectedRows.Count;
                        string id = "";
                        //遍历
                        for (int i = 0; i < count; i++)
                        {
                            int busid = int.Parse(this.gdvUnLoadCarMange.SelectedRows[i].Cells["BusinessRecord_ID"].Value.ToString());
                            Expression<Func<BusinessRecord, bool>> funuserinfo = n => n.BusinessRecord_ID == busid;
                            string strContent = LinQBaseDao.GetSingle("select smallTicket_Serialnumber from View_BusinessRecord_CarInfo where BusinessRecord_ID=" + busid).ToString();
                            if (BusinessRecordDAL.DeleteToMany(funuserinfo))
                            {
                                j++;
                                CommonalityEntity.WriteLogData("删除", "删除车牌号为：" + strContent + "，流水号为：" + strContent + "的过磅信息", CommonalityEntity.USERNAME);//添加操作日志
                            }
                        }
                        if (j == count)
                        {
                            MessageBox.Show("成功删除", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("删除失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                else//没有选中
                {
                    MessageBox.Show("请选择要删除的行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch 
            {

                CommonalityEntity.WriteTextLog("卸货点管理 tsbtnDel_Click()+" + "".ToString());
            }
            finally
            {
                LogInfoLoad("");
            }
        }
        /// <summary>
        /// 单节事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gdvUnLoadCarMange_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                int a = e.RowIndex;
                if (!Convert.ToBoolean(this.gdvUnLoadCarMange.Rows[a].Cells[0].Value))
                {
                    this.gdvUnLoadCarMange.Rows[a].Selected = true;
                    this.gdvUnLoadCarMange.Rows[a].Cells[0].Value = true;
                }
                else
                {
                    this.gdvUnLoadCarMange.Rows[a].Selected = false;
                    this.gdvUnLoadCarMange.Rows[a].Cells[0].Value = false;
                }
            }
            catch 
            {

            }

        }

        private void tsbExecl_Click()
        {
            string fileName = "卸货点车辆过磅Excel报表-" + CommonalityEntity.GetServersTime().ToShortDateString();
            tslExport_Excel(fileName, gdvUnLoadCarMange);
        }
        /// <summary>
        /// 导出Excel 的方法
        /// </summary>
        private void tslExport_Excel(string fileName, DataGridView myDGV)
        {
            string saveFileName = "";
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls";
            saveDialog.Filter = "Excel文件|*.xls";
            saveDialog.FileName = fileName;
            saveDialog.ShowDialog();
            saveFileName = saveDialog.FileName;
            if (saveFileName.IndexOf(":") < 0) return; //被点了取消 
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("无法创建Excel对象，可能您的机子未安装Excel");
                return;
            }

            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1 
            Microsoft.Office.Interop.Excel.Range range = worksheet.get_Range("A1", "Z" + (myDGV.SelectedRows.Count + 10));//把Execl设置问文本格式
            range.NumberFormatLocal = "@";

            //写入标题
            for (int i = 1; i < myDGV.ColumnCount; i++)
            {
                worksheet.Cells[1, i] = myDGV.Columns[i].HeaderText;
            }
            //写入数值
            int s = 0;
            for (int r = 0; r < myDGV.Rows.Count; r++)
            {
                if (Convert.ToBoolean(myDGV.Rows[r].Cells[0].Value))
                {
                    for (int i = 1; i < myDGV.ColumnCount; i++)
                    {
                        worksheet.Cells[s + 2, i] = myDGV.Rows[r].Cells[i].Value;
                    }
                    System.Windows.Forms.Application.DoEvents();
                    s++;
                }
            }
            worksheet.Columns.EntireColumn.AutoFit();//列宽自适应
            Microsoft.Office.Interop.Excel.Range rang = worksheet.get_Range(worksheet.Cells[2, 1], worksheet.Cells[myDGV.Rows.Count + 2, 2]);
            rang.NumberFormat = "000000000000";

            if (saveFileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFileName);
                    //fileSaved = true;
                }
                catch 
                {
                    //fileSaved = false;
                    MessageBox.Show("导出文件时出错,文件可能正被打开！\n" + "");
                }

            }
            //else
            //{
            //    fileSaved = false;
            //}
            xlApp.Quit();
            GC.Collect();//强行销毁 
            MessageBox.Show(fileName + ",保存成功", "提示", MessageBoxButtons.OK);

        }
    }
}
